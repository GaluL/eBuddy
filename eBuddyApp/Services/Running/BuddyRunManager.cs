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
using Windows.Services.Maps;
using eBuddyApp;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
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

        public MapRoute BuddyRoute
        {
            get { return _buddyRoute; }
            private set
            {
                _buddyRoute = value;
                OnBuddyRouteUpdate?.Invoke(value);
            }
        }

        private static BuddyRunManager _Instance;
        public static BuddyRunManager Instance => _Instance ?? (_Instance = new BuddyRunManager());

        private RunItem _BuddyData;

        public RunItem BuddyData
        {
            get { return _BuddyData; }
            private set { _BuddyData = value; }
        }

        public BuddyRunManager() : base()
        {
            BuddyData = new RunItem();
            _buddyWaypoints = new ObservableCollection<Geopoint>();
            _routeFinderEvent = new ManualResetEvent(true);
            LocationService.Instance.OnLocationChange += My_OnLocationChange;
        }

        private void My_OnLocationChange(Windows.Devices.Geolocation.Geoposition obj)
        {
            var msg = BuddyRunInfo.FromGeoposition(obj, DateTime.UtcNow);
            msg.SourceUserId = eBuddyApp.Services.Azure.MobileService.Instance.UserData.FacebookId;
            msg.DestUserId = "sid:af7d6ae6d4abbcb585bc46ab45d42c05"; //todo change to real usrid

            RunnersHubProxy.Invoke("SendLocation", msg);
        }


        private async void OnLocationMessage(BuddyRunInfo obj)
        {
            OnBuddyLocationUpdate?.Invoke(obj.GetGeoPoint());

            _routeFinderEvent.Reset();

            _buddyWaypoints.Add(obj.GetGeoPoint());

            if (_buddyWaypoints.Count > 1)
            {
                var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_buddyWaypoints);

                if (routeFind.Status == MapRouteFinderStatus.Success)
                {
                    BuddyRoute = routeFind.Route;
                    double distanceDiff = BuddyRoute.LengthInMeters - BuddyData.Distance;
                    BuddyData.Distance = BuddyRoute.LengthInMeters;
                    BuddyData.Speed = (distanceDiff / 1000) / ((BuddyData.Time.Subtract(DateTime.Now.TimeOfDay)).TotalSeconds / 60.0 / 60.0);
                    BuddyData.Time = DateTime.Now - BuddyData.Date;
                }
            }

            _routeFinderEvent.Set();
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

            await RunnersHubProxy.Invoke("HandShake","shiran6",
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
            Busy.SetBusy(true, "waiting for buddy approval");
            await ConnectHub();
        }

        public event Action<Geopoint> OnBuddyLocationUpdate;
    }
}
