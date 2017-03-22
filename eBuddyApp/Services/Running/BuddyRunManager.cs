using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using eBuddyApp;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using eBuddyApp.ViewModels;
using eBuddyApp.Views;
using Microsoft.WindowsAzure.MobileServices;
using Template10.Common;
using Microsoft.AspNet.SignalR.Client;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;

namespace eBuddy
{
    class BuddyRunManager : RunManager

    {
        private HubConnection RunnersHubConnection { get; set; }
        private IHubProxy RunnersHubProxy { get; set; }

        private readonly ObservableCollection<Geopoint> _buddyWaypoints;

        private ManualResetEvent _routeFinderEvent;

        private int _buddyLastLocationTimeSeconds = 1;


        public ObservableCollection<Geopoint> BuddyWaypoints
        {
            get { return _buddyWaypoints; }
        }

        public event Action<MapRoute> OnBuddyRouteUpdate;
        private MapRoute _buddyRoute;

        public event EventHandler RunAboutToStart;

        public MapRoute BuddyRoute
        {
            get { return _buddyRoute; }
            private set
            {
                _buddyRoute = value;
                OnBuddyRouteUpdate?.Invoke(value);
            }
        }

        private string _socialMsg;
        public string SocialMsg
        {
            get { return _socialMsg; }
            set
            {
                _socialMsg = value;
                OnMsgUpdate?.Invoke(value);
            }
        }

        private static BuddyRunManager _Instance;
        public static BuddyRunManager Instance => _Instance ?? (_Instance = new BuddyRunManager());

        private double RUN_KM = 1200;
        private string winner = "";
 //       string BUDDY_USER_ID = "sid:a750a0f0db72b4ac1dba1255de64cfb9"; //todo change to real usrid
        string BUDDY_USER_ID = "sid:af7d6ae6d4abbcb585bc46ab45d42c05"; //todo change to real usrid

        private RunItem _buddyRunData;
        public RunItem BuddyRunData
        {
            get { return _buddyRunData; }
            private set { _buddyRunData = value; }
        }

        private UserItem _buddyData;
        private bool inTalk;

        public UserItem BuddyData
        {
            get { return _buddyData; }
            private set { _buddyData = value; }
        }

        public event Action<string> OnMsgUpdate;
        public event Action<Color> OnMsgColorUpdate;
        public event Action<int> OnMsgSizeUpdate;

        public event Action<Geopoint> OnBuddyLocationUpdate;
        public event Action<Geopoint> OnBuddyFinish;



        public BuddyRunManager() : base()
        {
            BuddyRunData = new RunItem();
            _buddyWaypoints = new ObservableCollection<Geopoint>();
            _routeFinderEvent = new ManualResetEvent(true);
        }

        private async void GetBuddyData(string buddyUserId)
        {
            var userItems = await MobileService.Instance.Service.GetTable<UserItem>().Where(x => x.FacebookId == buddyUserId).ToCollectionAsync();

            if (userItems.Count != 0)
            {
                BuddyData = userItems[0];
            }
            else
            {
                throw new Exception("Buddy user was not found!");
            }
        }

        private void My_OnLocationChange(Windows.Devices.Geolocation.Geoposition obj)
        {
            if (RunData.Distance >= RUN_KM && winner.Equals(""))
            {
                winner = "me";
                MessageDialog dialog = new MessageDialog("You are the winner!"); //TODO

            }
            var msg = BuddyRunInfo.FromGeoposition(obj, DateTime.UtcNow);
            msg.SourceUserId = eBuddyApp.Services.Azure.MobileService.Instance.UserData.FacebookId;
            msg.DestUserId = BUDDY_USER_ID;

            RunnersHubProxy.Invoke("SendLocation", msg);
        }


        private async void OnLocationMessage(BuddyRunInfo obj)
        {
            OnBuddyLocationUpdate?.Invoke(obj.GetGeoPoint());

            _routeFinderEvent.Reset();

            _buddyWaypoints.Add(obj.GetGeoPoint());

            if (BuddyRunData.Distance >= RUN_KM && winner.Equals("") || true)
            {
                winner = "buddy";
                OnMsgColorUpdate?.Invoke(Colors.White);
                string he_she = "she";
                SocialMsg = BuddyData.PrivateName + " has completed the run and " + he_she + " is the winner!";
                OnMsgSizeUpdate?.Invoke(18);
            }

            if (_buddyWaypoints.Count > 1)
            {
                var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_buddyWaypoints);

                if (routeFind.Status == MapRouteFinderStatus.Success)
                {
                    BuddyRoute = routeFind.Route;
                    double distanceDiff = BuddyRoute.LengthInMeters - BuddyRunData.Distance;
                    BuddyRunData.Distance = BuddyRoute.LengthInMeters;
                    BuddyRunData.Speed = (distanceDiff / 1000) / ((BuddyRunData.Time.Subtract(DateTime.Now.TimeOfDay)).TotalSeconds / 60.0 / 60.0);
                    BuddyRunData.Time = DateTime.Now - BuddyRunData.Date;

                    if (BuddyRunData.Distance >= RUN_KM && winner.Equals(""))
                    {
                        winner = "buddy";
                        OnMsgColorUpdate?.Invoke(Colors.LightCoral);
                        string he_she = BuddyData.Gender == true ? "he" : "she";
                        SocialMsg = BuddyData.PrivateName + " has completed the run and " + he_she + " is the winner!";
                        await ReadText(SocialMsg);
                        OnMsgSizeUpdate?.Invoke(18);
                    }
                    else
                    {
                        UpdateTipMsg();
                    }
                }
            }

            _routeFinderEvent.Set();
        }

        public async Task ReadText(string text)
        {
            inTalk = true;
            MediaElement mediaPlayer = new MediaElement();
            using (var speach = new SpeechSynthesizer())
            {
                speach.Voice = SpeechSynthesizer.AllVoices.First(i => (i.Gender == VoiceGender.Female));
                SpeechSynthesisStream stream = await speach.SynthesizeTextToStreamAsync(text);
                mediaPlayer.SetSource(stream, stream.ContentType);
                mediaPlayer.Play();
            }
            inTalk = false;
        }


        private async void UpdateTipMsg()
        {
            OnMsgColorUpdate?.Invoke(Colors.DarkSlateGray);
            string he_she = BuddyData.Gender == true ? "he" : "she";

            if ((RunData.Distance < BuddyRunData.Distance) &&
                    (RunData.Speed > BuddyRunData.Speed))
            {
                double time_seconds = ((RUN_KM - BuddyRunData.Distance) / (BuddyRunData.Speed)) * 60 * 60;
                double tip_kmh = RUN_KM - RunData.Distance / time_seconds;
                OnMsgColorUpdate?.Invoke(Colors.Black);
                OnMsgSizeUpdate?.Invoke(17);
                if (tip_kmh < BuddyRunData.Speed)
                    SocialMsg = BuddyData.PrivateName + " is currently the first! speed up to " + tip_kmh +
                                " in order to win!";
                else

                    SocialMsg = BuddyData.PrivateName + " is currently the first! your speed is great and you're on the right track to win this race!";

            }
            else if ((RunData.Distance >= BuddyRunData.Distance) && (BandService.Instance.HeartRate < BandService.Instance.MinTargetZoneHeartRate))
            {
                OnMsgColorUpdate?.Invoke(Colors.DarkSlateGray);
                OnMsgSizeUpdate?.Invoke(15);
                SocialMsg = "Good job " + MobileService.Instance.UserData.PrivateName +
                            "! you are currently first! but you are not in your target Heart rate (that is " + BandService.Instance.MinTargetZoneHeartRate + " and up). fasten up!";
            }
            else if ((RunData.Distance >= BuddyRunData.Distance) &&
                     (BandService.Instance.HeartRate > BandService.Instance.MinTargetZoneHeartRate))
            {
                OnMsgColorUpdate?.Invoke(Colors.DarkGreen);
                OnMsgSizeUpdate?.Invoke(15);
                SocialMsg = "Good job " + MobileService.Instance.UserData.PrivateName +
                            "! you are currently first! and you are in your target Heart Rate!";
            }
            await ReadText(SocialMsg);


        }

        internal async Task<bool> ConnectHub()
        {
            RunnersHubConnection =
                new HubConnection(eBuddyApp.Services.Azure.MobileService.Instance.Service.MobileAppUri.AbsoluteUri);
            RunnersHubProxy = RunnersHubConnection.CreateHubProxy("SocialRunsHub");

            if (eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser != null)
            {
                RunnersHubConnection.Headers["x-zumo-auth"] =
                    eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.MobileServiceAuthenticationToken;
            }
            else
            {
                RunnersHubConnection.Headers["x-zumo-application"] = "";
            }

            await RunnersHubConnection.Start();


            if (RunnersHubConnection.State != ConnectionState.Connected)
            {
                return false;
            }

            RunnersHubProxy.On<string>("runStart", OnHandShake);

            RunnersHubProxy.On<BuddyRunInfo>("buddyLocationUpdate", OnLocationMessage);

            await RunnersHubProxy.Invoke("Register",
                eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId);

            await RunnersHubProxy.Invoke("HandShake", "shiran6",
                eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId);

            return true;
        }

        private void OnHandShake(string obj)
        {
            Busy.SetBusy(false);
            base.Start();
        }


        internal override async void Start()
        {
            GetBuddyData(BUDDY_USER_ID);
            InRun = true;
            LocationService.Instance.OnLocationChange += My_OnLocationChange;
            Busy.SetBusy(true, "waiting for buddy approval");
            await ConnectHub();
            SocialMsg = "RUN!";
            OnMsgColorUpdate?.Invoke(Colors.Green);
            OnMsgSizeUpdate?.Invoke(28);

        }

        internal override async void Stop()
        {
            InRun = false;
            base.Stop();
            LocationService.Instance.OnLocationChange -= My_OnLocationChange;
            //await RunnersHubProxy.Invoke("Stop",
                //eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId); //Todo implement server logic
            winner = "";


        }

        public void OnUpcomingRun(string facebookId)
        {
            BUDDY_USER_ID = facebookId;

            GetBuddyData(BUDDY_USER_ID);

            RunAboutToStart?.Invoke(this, null);
        }
    }
}
