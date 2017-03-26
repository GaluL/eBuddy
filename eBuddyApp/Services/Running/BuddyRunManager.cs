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
using Microsoft.Band;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;

namespace eBuddy
{
    class BuddyRunManager : RunManager

    {
        private HubConnection RunnersHubConnection { get; set; }
        private IHubProxy RunnersHubProxy { get; set; }

        private readonly ObservableCollection<Geopoint> _buddyWaypoints;

        private ManualResetEvent _routeFinderEvent;

        public ObservableCollection<Geopoint> BuddyWaypoints => _buddyWaypoints;

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
        private static BuddyRunManager _instance;
        public static BuddyRunManager Instance => _instance ?? (_instance = new BuddyRunManager());

       // private readonly double runDistance = 10 / 1000;
        private string _winner = "";
        //       string BUDDY_USER_ID = "sid:a750a0f0db72b4ac1dba1255de64cfb9"; //todo change to real usrid
        string _buddyUserId = "sid:af7d6ae6d4abbcb585bc46ab45d42c05"; //todo change to real usrid

        private RunItem _buddyRunData;
        public RunItem BuddyRunData
        {
            get { return _buddyRunData; }
            private set { _buddyRunData = value; }
        }

        private string _runId;

        public string RunId
        {
            get { return _runId;} 
           
            set {
                SocialMsg = "Let's start running! press start as soon as you are ready";
                _runId = value;
            }
        }

        public double RunDistance { get; set; }

        private UserItem _buddyData;
        private bool _handshakeOnce = false;
        private int _tip = 0;

        public UserItem BuddyData
        {
            get { return _buddyData; }
            private set { _buddyData = value; }
        }

        public event Action<Geopoint> OnBuddyLocationUpdate;
        public event Action<string> OnMsgUpdate;
        public event Action<Color> OnMsgColorUpdate;
        public event Action<int> OnMsgSizeUpdate;



        public BuddyRunManager() : base()
        {
            RunId = "nobody"; //TODO change to real value
            RunDistance = 0;
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

        private async void My_OnLocationChange(Windows.Devices.Geolocation.Geoposition obj)
        {

            if (InRun)
            {
                if (RunData.Distance >= RunDistance && _winner.Equals("")) //todo
                {
                    OnMsgSizeUpdate?.Invoke(17);
                    _winner = MobileService.Instance.UserData.PrivateName;
                    OnMsgSizeUpdate?.Invoke(15);
                    OnMsgColorUpdate?.Invoke(Colors.RoyalBlue);
                    SocialMsg = "Great job " + MobileService.Instance.UserData.PrivateName +
                                " you have completed the run first and you are the winner! yay!";
                    Stop();
                }
                else
                {
                    var msg = BuddyRunInfo.FromGeoposition(obj, DateTime.UtcNow);
                    msg.SourceUserId = eBuddyApp.Services.Azure.MobileService.Instance.UserData.FacebookId;
                    msg.DestUserId = _buddyUserId;

                    await RunnersHubProxy.Invoke("SendLocation", msg);
                }
            }
        }

        public override void CallbackB(object state)
        {
            if (InRun && !RunData.Time.Minutes.Equals(0))
            {
                SocialMsg = "0Time: " + RunData.Time.Minutes + "minutes" + RunData.Time.Seconds + "seconds . Distance: " +
                           RunData.Distance
                           + " meters. Speed: " + RunData.Speed
                           + "kilometer per hour";
            }
        }

        private async void OnLocationMessage(BuddyRunInfo obj)
        {
            if (InRun)
            {
                OnBuddyLocationUpdate?.Invoke(obj.GetGeoPoint());

                _routeFinderEvent.Reset();

                _buddyWaypoints.Add(obj.GetGeoPoint());

                UpdateTipMsg();

                if (_buddyWaypoints.Count > 1)
                {
                    var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_buddyWaypoints);

                    if (routeFind.Status == MapRouteFinderStatus.Success)
                    {
                        BuddyRoute = routeFind.Route;
                        double distanceDiff = BuddyRoute.LengthInMeters - BuddyRunData.Distance;
                        BuddyRunData.Distance = BuddyRoute.LengthInMeters;
                        BuddyRunData.Speed = (distanceDiff / 1000) /
                                             ((BuddyRunData.Time.Subtract(DateTime.Now.TimeOfDay)).TotalSeconds / 60.0 /
                                              60.0);
                        BuddyRunData.Time = DateTime.Now - BuddyRunData.Date;

                        if (BuddyRunData.Distance >= RunDistance && _winner.Equals(""))
                        {
                            _winner = BuddyData.PrivateName;
                            OnMsgColorUpdate?.Invoke(Colors.LightCoral);
                            string he_she = BuddyData.Gender == true ? "he" : "she";
                            SocialMsg = BuddyData.PrivateName + " has completed the run and " + he_she +
                                        " is the winner!";
                            OnMsgSizeUpdate?.Invoke(18);
                        }

                    }
                }

                _routeFinderEvent.Set();
            }
        }

        private void OnBuddyFinish(BuddyRunInfo obj)
        {
            if (InRun)
            {
                SocialMsg = "Buddy has finished his run";
            }


        }

        public void UpdateTipMsg()
        {
            if (RunData.Distance < BuddyRunData.Distance)
            {
                
                double timeSeconds = ((((RunDistance - BuddyRunData.Distance)/1000) / (BuddyRunData.Speed > 0.5 ? BuddyRunData.Speed : 1)) * 60 * 60);
                double tipKmh = (RunDistance - RunData.Distance) / timeSeconds;
                OnMsgColorUpdate?.Invoke(Colors.Black);
                OnMsgSizeUpdate?.Invoke(17);
                if (tipKmh < BuddyRunData.Speed && _tip != 1)
                {
                    SocialMsg = BuddyData.PrivateName + " is currently the first! speed up to " + tipKmh +
                                " in order to win!";
                    _tip = 1;
                }
                else if (tipKmh >= BuddyRunData.Speed && _tip != 2)
                {

                    SocialMsg = BuddyData.PrivateName +
                                " is currently first! your speed is great and you're on the right track to win this race!";
                    _tip = 2;

                }
            }
            else if ((RunData.Distance >= BuddyRunData.Distance) &&
                     (BandService.Instance.HeartRate < BandService.Instance.MinTargetZoneHeartRate) && _tip != 3)
            {
                OnMsgColorUpdate?.Invoke(Colors.DarkSlateGray);
                OnMsgSizeUpdate?.Invoke(15);
                SocialMsg = "Good job " + MobileService.Instance.UserData.PrivateName +
                            "! you are currently first! but you are not in your target Heart rate (that is " +
                            BandService.Instance.MinTargetZoneHeartRate + " and up). speed up!";
                _tip = 3;
            }
            else if ((RunData.Distance >= BuddyRunData.Distance) &&
                     (BandService.Instance.HeartRate > BandService.Instance.MinTargetZoneHeartRate) && _tip != 4)
            {
                OnMsgColorUpdate?.Invoke(Colors.DarkGreen);
                OnMsgSizeUpdate?.Invoke(15);
                SocialMsg = "Good job " + MobileService.Instance.UserData.PrivateName +
                            "! you are currently first! and you are in your target Heart Rate!";
                _tip = 4;
            }


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

            RunnersHubProxy.On<BuddyRunInfo>("BuddyFinish", OnBuddyFinish);


            await RunnersHubProxy.Invoke("Register",
                eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId);

            await RunnersHubProxy.Invoke("HandShake", RunId,
                eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId);

            return true;
        }

        private void OnHandShake(string obj)
        {
            if (!_handshakeOnce)
            {
                Busy.SetBusy(false);
                base.Start();
                OnMsgSizeUpdate?.Invoke(28);
                SocialMsg = "Social run started";
                OnMsgColorUpdate?.Invoke(Colors.White);

                _handshakeOnce = true;
            }
        }


        internal override async void Start()
        {
            _handshakeOnce = false;
            solorun = false;
            _tip = 0;
            InRun = true;
            LocationService.Instance.OnLocationChange += My_OnLocationChange;
            Busy.SetBusy(true, "waiting for buddy approval");
            await ConnectHub();
            //SocialMsg = "RUN!";
            //OnMsgColorUpdate?.Invoke(Colors.Green);
            //OnMsgSizeUpdate?.Invoke(28);
        }

        internal override async void Stop()
        {
            RunData.Speed = RunData.Time.Seconds != 0 ? (RunData.Distance / 1000) / (RunData.Time.Seconds / 60.0 / 60) : 0;
            if (double.IsNaN(RunData.Speed)) RunData.Speed = 0;
            SocialMsg = "0activity completed, " + _winner + " is the winner!. Run summery. Time: " + RunData.Time.Minutes + "minutes" + RunData.Time.Seconds +
           "seconds . Distance: " + RunData.Distance
           + " meters. Average speed: " + RunData.Speed
           + "kilometer per hour";
            RunManager.Instance.RunData = RunData;
            OnFinish?.Invoke(0);
            InRun = false;
            await RunnersHubProxy.Invoke("BuddyFinish", RunId,
                eBuddyApp.Services.Azure.MobileService.Instance.Service.CurrentUser.UserId);
            _winner = "";
            solorun = true;
            BuddyWaypoints.Clear();
            BuddyRunData.Distance = 0;
            BuddyRunData.Speed = 0;
            RunId = "";
            RunDistance = 0; 
            base.Stop();
        }

        public void OnUpcomingRun(string facebookId, string runId, double distance)
        {
            _buddyUserId = facebookId;

            RunId = runId;

            RunDistance = distance * 1000;

            GetBuddyData(_buddyUserId);

            SocialMsg = "Let's start running! press start as soon as you are ready";

            RunAboutToStart?.Invoke(this, null);
        }

        public event Action<int> OnFinish;
    }
}
