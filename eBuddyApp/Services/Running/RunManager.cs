using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Microsoft.WindowsAzure.MobileServices;
using Template10.Common;

namespace eBuddy
{
    class RunManager
    {
        private int _lastLocationTimeSeconds = 1;
        private static RunManager _Instance;
        public static RunManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new RunManager();   
                }

                return _Instance;
            }
        }

        private RunItem _RunData;
        public RunItem RunData
        {
            get { return _RunData; }
            private set { _RunData = value; }
        }

        public bool InRun = false;

        private IList<Geopoint> _Waypoints;

        protected ManualResetEvent _DataUpdateSyncEvent;

        internal event EventHandler<MapRoute> OnRouteUpdate; 

        public RunManager()
        {
            _DataUpdateSyncEvent = new ManualResetEvent(true);

            RunData = new RunItem();
            RunData.FacebookId = MobileService.Instance.UserData.FacebookId;
        }

        public Timer aTimer;


        internal virtual void Start()
        {
            InRun = true;
            aTimer = new Timer(Callback, null, 0, 1); 
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;

            RunData.Date = DateTime.Now;
            _Waypoints = new List<Geopoint>();

            LocationService.Instance.Start();
        }

        private void Callback(object state)
        {
            RunData.Time = DateTime.Now - RunData.Date;

        }

        internal virtual void Stop()
        {
            InRun = false;
            aTimer.Dispose();
            LocationService.Instance.Stop();
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;

            MobileService.Instance.SaveRunData(RunData);
        }

        protected virtual async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            await UpdateRunStats(obj);

            _DataUpdateSyncEvent.Set();
        }

        protected async Task UpdateRunStats(Geoposition obj)
        {
            _Waypoints.Add(obj.ToGeoPoint());

            var route = await MapServiceWrapper.Instance.GetRoute(_Waypoints);

            if (route != null)
            {
                OnRouteUpdate?.Invoke(this, route);
                double distanceDiff = route.LengthInMeters - RunData.Distance;
                RunData.Distance = route.LengthInMeters;
                RunData.Speed = (distanceDiff / 1000) / (_lastLocationTimeSeconds / 60.0 / 60.0);
                _lastLocationTimeSeconds = RunData.Time.Seconds;

            }
}
    }
}
