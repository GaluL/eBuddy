using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace eBuddy
{
    class LocationTracker
    {
        private static LocationTracker _instance;
        public static LocationTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocationTracker();
                }

                return _instance;
            }
        }

        private static Geolocator _geolocator;

        private PositionStatus _locationStatus;
        public PositionStatus LocationStatus
        {
            get { return _locationStatus; }
            private set
            {
                _locationStatus = value;
                OnLocationStatusChange?.Invoke(this, null);
            }
        }

        private Geoposition _currentLocation;
        public Geoposition CurrentLocation
        {
            get { return _currentLocation; }
            private set
            {
                _currentLocation = value;
                OnLocationChange?.Invoke(value);
            }
        }

        public event EventHandler OnLocationStatusChange;
        public event Action<Geoposition> OnLocationChange;

        private LocationTracker()
        {
            _geolocator = new Geolocator();

            _geolocator.ReportInterval = 1000;

            // Subscribe to the StatusChanged event to get updates of location status changes.
            _geolocator.StatusChanged += OnStatusChanged;
        }

        public void Start()
        {
            if (_geolocator != null)
            {
                // Subscribe to the PositionChanged event to get location updates.
                _geolocator.PositionChanged += OnPositionChanged;
            }
        }

        public void Stop()
        {
            if (_geolocator != null)
            {
                // Subscribe to the PositionChanged event to get location updates.
                _geolocator.PositionChanged -= OnPositionChanged;
            }
        }

        private void UpdateLocationData(Geoposition pos)
        {
            CurrentLocation = pos;
        }

        async private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            LocationStatus = e.Status;
        }

        async private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            UpdateLocationData(e.Position);
        }
    }
}
