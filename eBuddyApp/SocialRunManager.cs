using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Microsoft.AspNet.SignalR.Client;

namespace eBuddy
{
    class SocialRunManager : RunManager
    {
        private HubConnection runnersHubConnection { get; set; }
        private IHubProxy runnersHubProxy { get; set; }

        private ObservableCollection<Geopoint> _buddyWaypoints;

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

        public SocialRunManager() : base()
        {
            LocationTracker.Instance.OnLocationChange += Instance_OnLocationChange;
        }

        private void Instance_OnLocationChange(Windows.Devices.Geolocation.Geoposition obj)
        {
            var msg = LocationMessage.FromGeoposition(obj, DateTime.UtcNow);
            msg.SourceUserId = App.MobileService.CurrentUser.UserId;
            msg.DestUserId = "sid:af7d6ae6d4abbcb585bc46ab45d42c05";

            runnersHubProxy.Invoke("SendLocation", msg);
        }

        //internal async void RegisterToUpdates()
        //{
        //    runnersHubProxy.Invoke("Register", App.MobileService.CurrentUser.UserId);

        //    runnersHubProxy.On<LocationMessage>("buddyLocationUpdate", OnLocationMessage);
        //}

        private async void OnLocationMessage(LocationMessage obj)
        {
            _buddyWaypoints.Add(obj.GetGeoPoint());

            var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_buddyWaypoints);

            if (routeFind.Status == MapRouteFinderStatus.Success)
            {
                BuddyRoute = routeFind.Route;
            }
        }

        internal async Task<bool> ConnectHub()
        {
            runnersHubConnection = new HubConnection(App.MobileService.MobileAppUri.AbsoluteUri);
            runnersHubProxy = runnersHubConnection.CreateHubProxy("SocialRunsHub");

            if (App.MobileService.CurrentUser != null)
            {
                runnersHubConnection.Headers["x-zumo-auth"] = 
                    App.MobileService.CurrentUser.MobileServiceAuthenticationToken;
            }
            else
            {
                runnersHubConnection.Headers["x-zumo-application"] = "";
            }

            await runnersHubConnection.Start();

            if (runnersHubConnection.State != ConnectionState.Connected)
            {
                return false;
            }

            await runnersHubProxy.Invoke("Register", App.MobileService.CurrentUser.UserId);

            runnersHubProxy.On<LocationMessage>("buddyLocationUpdate", OnLocationMessage);

            return true;
        }
    }
}
