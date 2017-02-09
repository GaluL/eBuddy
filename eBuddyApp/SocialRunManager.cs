using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Microsoft.AspNet.SignalR.Client;

namespace eBuddy
{
    class SocialRunManager : RunManager
    {
        public HubConnection runnersHubConnection { get; set; }
        public IHubProxy runnersHubProxy { get; set; }

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
            ConnectHub();

            LocationTracker.Instance.OnLocationChange += Instance_OnLocationChange;
        }

        private void Instance_OnLocationChange(Windows.Devices.Geolocation.Geoposition obj)
        {
            var msg = LocationMessage.FromGeoposition(obj, DateTime.UtcNow);
            msg.SourceUserId = App.MobileService.CurrentUser.UserId;
            msg.DestUserId = "";

            runnersHubProxy.Invoke("SendLocation", msg);
        }

        internal async void RegisterToUpdates()
        {
            runnersHubProxy.Invoke("Register", App.MobileService.CurrentUser.UserId);

            runnersHubProxy.On<LocationMessage>("buddyLocationUpdate", OnLocationMessage);
        }

        private async void OnLocationMessage(LocationMessage obj)
        {
            _buddyWaypoints.Add(obj.GetGeoPoint());

            var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_buddyWaypoints);

            if (routeFind.Status == MapRouteFinderStatus.Success)
            {
                BuddyRoute = routeFind.Route;
            }
        }

        internal async void ConnectHub()
        {
            runnersHubConnection = new HubConnection("http://ebuddy.azurewebsites.net");
            runnersHubProxy = runnersHubConnection.CreateHubProxy("SocialRunsHub");

            await runnersHubConnection.Start();
        }
    }
}
