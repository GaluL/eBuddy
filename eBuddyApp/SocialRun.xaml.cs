<<<<<<< HEAD
=======

>>>>>>> ee401ae3bc6810330432c2f42f9e6d4b00d3cf15
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
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

<<<<<<< HEAD
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
=======
// The UserItem Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
>>>>>>> ee401ae3bc6810330432c2f42f9e6d4b00d3cf15

namespace eBuddy
{
    public sealed partial class SocialRun : UserControl
    {
        private MainPage rootPage;

        private RunManager _runManager;
        private ManualResetEvent _mapServiceEvent;
        private bool _scoreRun;

        public SocialRun()
        {
            this.InitializeComponent();
            _scoreRun = false;

            MapService.ServiceToken =
                "OARdWd6u76SCOJpF63br~nEfdGL_BYBbFn1jt0wom8Q~Aoyg4vAPbQczjy1VVSUyFfFcpz_G1Q9eqrBUO9FdMP1a725us7XvhB7zycvi-lbq";
        }

        private async void _runManager_OnTimeUpdate(TimeSpan obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    timeDataLabel.Text = obj.ToString(@"mm\:ss");
                }
                );
        }

        private async void _runManager_OnSpeedUpdate(double obj)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (obj.ToString("00.0")[0] == '0')
                            speedDataLabel.Text = obj.ToString("0.0") + " min/km";
                        else
                            speedDataLabel.Text = obj.ToString("00.0") + " min/km";

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
                                distanceDataLabel.Text = obj.ToString("0.00") + " km";
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
                myMap.ZoomLevel = 17;
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

        private async void ButtonGo_OnClick(object sender, RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (_scoreRun)
            {
                _runManager = new TestRunManager(); //ofek
                _runManager.OnRouteUpdate += _runManager_OnRouteUpdate;
                _runManager.OnDistanceUpdate += _runManager_OnDistanceUpdate;
                _runManager.OnHeartRateUpdate += _runManager_OnHeartRateUpdate;
                _runManager.OnSpeedUpdate += _runManager_OnSpeedUpdate;
                _runManager.OnTimeUpdate += _runManager_OnTimeUpdate;
                ((TestRunManager)_runManager).OnStatusChanged += _runManager_OnStatusChanged;
                ((TestRunManager)_runManager).OnMASScoreUpdate += _runManager_OnMASScoreUpdate;
                ((TestRunManager)_runManager).OnVo2Update += _runManager_OnVo2Update;
            }
            else
            {
                _runManager = new RunManager();
                _runManager.OnRouteUpdate += _runManager_OnRouteUpdate;
                _runManager.OnDistanceUpdate += _runManager_OnDistanceUpdate;
                _runManager.OnHeartRateUpdate += _runManager_OnHeartRateUpdate;
                _runManager.OnSpeedUpdate += _runManager_OnSpeedUpdate;
                _runManager.OnTimeUpdate += _runManager_OnTimeUpdate;
            }
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                _runManager.Start();
            }
        }

        private async void _runManager_OnVo2Update(double obj)
        {
            MessageDialog dialog = new MessageDialog("Vo2: " + ((TestRunManager)_runManager).V02Max.ToString());
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }

        private async void _runManager_OnMASScoreUpdate(double obj)
        {
            MessageDialog dialog = new MessageDialog("Score: " + ((TestRunManager)_runManager).MASscore.ToString());
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }

        private async void _runManager_OnStatusChanged(TestRunManager.ETestPhase obj)
        {
            MessageDialog dialog = null;
            switch (obj)
            {
                case TestRunManager.ETestPhase.Chill:
                    dialog = new MessageDialog("Chill for 5 seconds - were calculating your rest heartbeat.");
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                    break;
                case TestRunManager.ETestPhase.WarmUp:
                    dialog = new MessageDialog("Start warm up for 20 minutes");
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                    break;
                case TestRunManager.ETestPhase.InRun:
                    dialog = new MessageDialog("Run your best time for 1200 meters");
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                    break;
                case TestRunManager.ETestPhase.Finished:
                    dialog = new MessageDialog("Finished! calculating your score..");
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                    break;
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

            _runManager.OnRouteUpdate -= _runManager_OnRouteUpdate;
            _runManager.OnDistanceUpdate -= _runManager_OnDistanceUpdate;
            _runManager.OnHeartRateUpdate -= _runManager_OnHeartRateUpdate;
            _runManager.OnSpeedUpdate -= _runManager_OnSpeedUpdate;
            _runManager.OnTimeUpdate -= _runManager_OnTimeUpdate;
        }

        private async void UpdateButtonStates(bool registered)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ButtonGo.IsEnabled = !registered;
                    //ButtonStop.IsEnabled = registered;
                });
        }

        private void scoreFlowVisibleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _scoreRun = false;
        }

        private void scoreFlowVisible_Checked(object sender, RoutedEventArgs e)
        {
            _scoreRun = true;
        }
    }
}

