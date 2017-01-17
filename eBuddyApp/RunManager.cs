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
using Microsoft.WindowsAzure.MobileServices;

namespace eBuddy
{
    class RunManager
    {
        private DateTime _startTime;
        private TimeSpan _time;
        private double _speed;
        private double _distance;
        private double _heartrate;
        private uint _elevationChange;
        private ManualResetEvent _mapServiceEvent;

        private ObservableCollection<Geopoint> _waypoints;
        public ObservableCollection<Geopoint> Waypoints
        {
            get { return _waypoints; }
        }

        public RunManager()
        {
            _waypoints = new ObservableCollection<Geopoint>();
            _mapServiceEvent = new ManualResetEvent(true);

            LocationTracker.Instance.OnLocationChange += LocationTracker_OnLocationChange;
            BandHandler.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
        }

        private void Instance_OnHeartRateChange(object sender, HeartRateSample e)
        {
            if (e != null)
            {
                Heartrate = e.HeartRate;
            }
        }

        public TimeSpan Time
        {
            get { return _time; }
            private set
            {
                _time = value;

                OnTimeUpdate?.Invoke(value);
            }
        }

        public double Speed
        {
            get { return _speed; }
            private set
            {
                _speed = value;

                OnSpeedUpdate?.Invoke(value);
            }
        }

        public double Distance
        {
            get { return _distance; }
            private set
            {
                _distance = value;

                OnDistanceUpdate?.Invoke(value);
            }
        }

        public double Heartrate
        {
            get { return _heartrate; }
            private set
            {
                _heartrate = value;

                OnHeartRateUpdate?.Invoke(value);
            }
        }

        public uint ElevationChange
        {
            get { return _elevationChange; }
            private set
            {
                _elevationChange = value;
            }
        }

        private MapRoute _route;
        public MapRoute Route
        {
            get { return _route; }
            private set
            {
                _route = value;

                OnRouteUpdate?.Invoke(value);
            }
        }

        public event Action<TimeSpan> OnTimeUpdate;
        public event Action<double> OnDistanceUpdate;
        public event Action<double> OnSpeedUpdate;
        public event Action<double> OnHeartRateUpdate;
        public event Action<MapRoute> OnRouteUpdate; 

        private int cnt = 0;

        private async void LocationTracker_OnLocationChange(Geoposition obj)
        {
            //_waypoints.Add(new Geopoint(new BasicGeoposition()
            //{
            //    Altitude = obj.Coordinate.Altitude.HasValue ? obj.Coordinate.Altitude.Value : 0,
            //    Latitude = obj.Coordinate.Latitude,
            //    Longitude = obj.Coordinate.Longitude
            //}));

            List<Geopoint> list = new List<Geopoint>()
            {
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.079864147217236, Longitude = 34.770158141831232}),
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.079905, Longitude = 34.769737}),
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.079969, Longitude = 34.768267}),
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.080014, Longitude = 34.76729}),
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.081532, Longitude = 34.76772}),
                new Geopoint(new BasicGeoposition() {Altitude = 56, Latitude = 32.082732, Longitude = 34.76876})
            };

            _waypoints.Add(list[cnt]);

            cnt = (cnt + 1) % 6;

            if (_waypoints.Count > 1)
            {
                _mapServiceEvent.Reset();

                var routeFind = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(_waypoints);

                if (routeFind.Status == MapRouteFinderStatus.Success)
                {
                    Route = routeFind.Route;
                    Time = DateTime.Now - _startTime;
                    Distance = Route.LengthInMeters;
                    Speed = (Distance / 1000.0) / (Time.Seconds / 60.0 / 60.0);
                }

                _mapServiceEvent.Set();
            }
        }

        internal void Start()
        {
            _waypoints.Clear();

            _startTime = DateTime.Now;
            LocationTracker.Instance.Start();
        }

        internal void Stop()
        {
            LocationTracker.Instance.Stop();
        }
    }
}
