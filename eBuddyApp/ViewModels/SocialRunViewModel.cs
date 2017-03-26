using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using eBuddyApp.Services.Weather;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;
using WeatherNet.Model;

namespace eBuddyApp.ViewModels
{
    class SocialRunViewModel : SoloRunViewModel
    {

        private MapRoute _BuddyRoute;

        public MapRoute BuddyRoute
        {
            get { return _BuddyRoute; }
            private set { Set(ref _BuddyRoute, value); }
        }

        private Geopoint _BuddyLocation;

        public Geopoint BuddyLocation
        {
            get { return _BuddyLocation; }
            private set { Set(ref _BuddyLocation, value); }
        }

        public override RunItem RunData
        {
            get { return BuddyRunManager.Instance.RunData; }
        }


        private string _socialMsg;

        public string SocialMsg
        {
            get { return _socialMsg; }
            set { Set(ref _socialMsg, value); }
        }

        private Color _socialColor;

        public Color SocialColor
        {
            get { return _socialColor; }
            set { Set(ref _socialColor, value); }
        }

        private int _socialSize;

        public int SocialSize
        {
            get { return _socialSize; }
            set { Set(ref _socialSize, value); }
        }

        public Page page;

        private CurrentWeatherResult _CurrentBuddyWeather;
        private ManualResetEvent routeEvent;

        public CurrentWeatherResult CurrentBuddyWeather
        {
            get { return _CurrentBuddyWeather; }
            set
            {
                _CurrentBuddyWeather = value;
                RaisePropertyChanged("CurrentBuddyWeather");
            }
        }

        public SocialRunViewModel()
        {
            routeEvent = new ManualResetEvent(true);
            SocialMsg = "You don't have any schedualed runs for now. Invite a buddy!";
            SocialColor = Colors.DarkGray;
            SocialSize = 16;
            StartRun = new RelayCommand(() =>
                {
                    BuddyRunManager.Instance.Start();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    BuddyRunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
                    BuddyRunManager.Instance.OnBuddyRouteUpdate += Instance_OnBuddyRouteUpdate;
                    BuddyRunManager.Instance.OnBuddyLocationUpdate += Instance_OnBuddyLocationUpdate;
                    BuddyRunManager.Instance.OnMsgUpdate += Instance_OnMsgVUpdate;
                    BuddyRunManager.Instance.OnMsgColorUpdate += Instance_OnMsgColorUpdate;
                    BuddyRunManager.Instance.OnMsgSizeUpdate += Instance_OnMsgSizeUpdate;
                    BuddyRunManager.Instance.OnFinish += Instance_OnFinish;

                },
                () => { return (!BuddyRunManager.Instance.InRun && !BuddyRunManager.Instance.RunId.Equals("")); });

            StopRun = new RelayCommand(() =>
                {
                    BuddyRunManager.Instance.Stop();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    BuddyRunManager.Instance.OnRouteUpdate -= Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                    BuddyRunManager.Instance.OnBuddyRouteUpdate -= Instance_OnBuddyRouteUpdate;
                    BuddyRunManager.Instance.OnBuddyLocationUpdate -= Instance_OnBuddyLocationUpdate;
                    BuddyRunManager.Instance.OnMsgUpdate -= Instance_OnMsgVUpdate;
                    BuddyRunManager.Instance.OnMsgColorUpdate -= Instance_OnMsgColorUpdate;
                    BuddyRunManager.Instance.OnMsgSizeUpdate -= Instance_OnMsgSizeUpdate;
                },
                () => { return BuddyRunManager.Instance.InRun; });

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
            BuddyLocation = ExtentionMethods.GetDefaultPoint();


        }

        private void Instance_OnFinish(int obj)
        {
            BuddyRunManager.Instance.OnRouteUpdate -= Instance_OnRouteUpdate;
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
            BuddyRunManager.Instance.OnBuddyRouteUpdate -= Instance_OnBuddyRouteUpdate;
            BuddyRunManager.Instance.OnBuddyLocationUpdate -= Instance_OnBuddyLocationUpdate;
            BuddyRunManager.Instance.OnMsgUpdate -= Instance_OnMsgVUpdate;
            BuddyRunManager.Instance.OnMsgColorUpdate -= Instance_OnMsgColorUpdate;
            BuddyRunManager.Instance.OnMsgSizeUpdate -= Instance_OnMsgSizeUpdate;
            BuddyRunManager.Instance.InRun = false;
            StopRun.RaiseCanExecuteChanged();
            StartRun.RaiseCanExecuteChanged();
        }

        private void Instance_OnMsgSizeUpdate(int obj)
        {
            SocialSize = obj;
        }

        private void Instance_OnMsgColorUpdate(Color obj)
        {
            SocialColor = obj;
        }

        private async void Instance_OnMsgVUpdate(string obj)
        {
            if (!obj[0].Equals('0'))
            {
                SocialMsg = obj;
            }
            else
            {
                obj = obj.Remove(0, 1);
            }
            await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
              {
                  await RunManager.Instance.ReadText(obj);
              });
        
        }



        private void Instance_OnBuddyRouteUpdate(MapRoute e)
        {
            BuddyRoute = e;
        }

        private void Instance_OnBuddyLocationUpdate(Geopoint obj)
        {
            BuddyLocation = obj;

            CurrentBuddyWeather = WeatherService.Instance.GetWeatherForLocation(obj.Position.Longitude,
                obj.Position.Latitude).Result;
        }


       

    }





}
