using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace eBuddy
{
    public sealed partial class RunTab : UserControl
   {
        private MainPage rootPage;

        private TestRunManager _runManager;
        private ManualResetEvent _mapServiceEvent;

        public RunTab()
        {
            this.InitializeComponent();

            _runManager = new TestRunManager();

            _runManager.OnRouteUpdate += _runManager_OnRouteUpdate;
            _runManager.OnDistanceUpdate += _runManager_OnDistanceUpdate;
            _runManager.OnHeartRateUpdate += _runManager_OnHeartRateUpdate;
            _runManager.OnSpeedUpdate += _runManager_OnSpeedUpdate;
            _runManager.OnTimeUpdate += _runManager_OnTimeUpdate;

            MapService.ServiceToken =
                "OARdWd6u76SCOJpF63br~nEfdGL_BYBbFn1jt0wom8Q~Aoyg4vAPbQczjy1VVSUyFfFcpz_G1Q9eqrBUO9FdMP1a725us7XvhB7zycvi-lbq";
        }

        private async void _runManager_OnTimeUpdate(TimeSpan obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    timeDataLabel.Text = obj.ToString(@"hh\:mm\:ss");
                }
                );
        }

        private async void _runManager_OnSpeedUpdate(double obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        speedDataLabel.Text = obj.ToString("0.00");
                    }
                    );
        }

        private async void _runManager_OnHeartRateUpdate(double obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () =>
                                        {
                                            heartrateDataLabel.Text = obj.ToString();
                                        }
                                        );
        }

        private async void _runManager_OnDistanceUpdate(double obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                distanceDataLabel.Text = obj.ToString();
                            }
                            );
        }

        private async void _runManager_OnRouteUpdate(MapRoute obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {

                double latitude = LocationTracker.Instance.CurrentLocation.Coordinate.Latitude;
                double longitude = LocationTracker.Instance.CurrentLocation.Coordinate.Longitude;
                myMap.Center = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude });
                myMap.ZoomLevel = 20;
                myMap.DesiredPitch = 64;
                var routeView = new MapRouteView(obj);

                myMap.Routes.Clear();
                myMap.Routes.Add(routeView);
            }
            );
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
                 
            myMap.Center = MainPage.SeattleGeopoint;
            myMap.ZoomLevel = 15;

        }

        private void MyMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            var tappedGeoPosition = args.Location.Position;
            string status = "MapTapped at \nLatitude:" + tappedGeoPosition.Latitude + "\nLongitude: " + tappedGeoPosition.Longitude;
            //rootPage.NotifyUser(status, NotifyType.StatusMessage);
        }

        private void TrafficFlowVisible_Checked(object sender, RoutedEventArgs e)
        {
            myMap.TrafficFlowVisible = true;
        }

        private void trafficFlowVisibleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            myMap.TrafficFlowVisible = false;
        }

//        private void styleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            switch (styleCombobox.SelectedIndex)
//            {
//                case 0:
//                    myMap.Style = MapStyle.None;
//                    break;
//                case 1:
//                    myMap.Style = MapStyle.Road;
//                    break;
//                case 2:
//                    myMap.Style = MapStyle.Aerial;
//                    break;
//                case 3:
//                    myMap.Style = MapStyle.AerialWithRoads;
//                    break;
//                case 4:
//                    myMap.Style = MapStyle.Terrain;
//                    break;
//            }
//        }

        // For background task registration
        private const string BackgroundTaskName = "SampleLocationBackgroundTask";
        private const string BackgroundTaskEntryPoint = "LocationTracker";

        private IBackgroundTaskRegistration _geolocTask = null;

        private async void ButtonGo_OnClick(object sender, RoutedEventArgs e)
        { 
            var accessStatus = await Geolocator.RequestAccessAsync();

            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                _runManager.Start();
            }
        }
        /// <summary>
        /// Get permission for location from the user. If the user has already answered once,
        /// this does nothing and the user must manually update their preference via Settings.
        /// </summary>
        private async void RequestLocationAccess()
        {
            // Request permission to access location
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            _runManager.Stop();
//            Frame.Navigate(typeof(BandPage), 1);
            //ScenarioOutput_Latitude.Text = "No data";
            //ScenarioOutput_Longitude.Text = "No data";
            //ScenarioOutput_Accuracy.Text = "No data";
            UpdateButtonStates(/*registered:*/ false);
            //_rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Event handle to be raised when the background task is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the error text box.
                        e.CheckResult();

                        // Update the UI with the completion status of the background task
                        // The Run method of the background task sets this status. 
                        var settings = ApplicationData.Current.LocalSettings;
                        if (settings.Values["Status"] != null)
                        {
                            //_rootPage.NotifyUser(settings.Values["Status"].ToString(), NotifyType.StatusMessage);
                        }

                        // Extract and display location data set by the background task if not null
                        //ScenarioOutput_Latitude.Text = (settings.Values["Latitude"] == null) ? "No data" : settings.Values["Latitude"].ToString();
                        //ScenarioOutput_Longitude.Text = (settings.Values["Longitude"] == null) ? "No data" : settings.Values["Longitude"].ToString();
                        //ScenarioOutput_Accuracy.Text = (settings.Values["Accuracy"] == null) ? "No data" : settings.Values["Accuracy"].ToString();
                    }
                    catch (Exception ex)
                    {
                        // The background task had an error
                        //_rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                    }
                });
            }
        }

        /// <summary>
        /// Update the enable state of the register/unregister buttons.
        /// 
        private async void UpdateButtonStates(bool registered)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ButtonGo.IsEnabled = !registered;
                    //ButtonStop.IsEnabled = registered;
                });
        }

    }
}

