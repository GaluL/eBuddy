using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using eBuddyApp.Services.Weather;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;
using WeatherNet.Model;

namespace eBuddyApp.ViewModels
{
    class SoloRunViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;

        #endregion

        #region Properties

        public Page page;
        private ManualResetEvent speechEvent;
        MediaElement mediaPlayer = new MediaElement();

        public bool IsScoreRun { get; set; }

        public virtual RunItem RunData
        {
            get { return RunManager.Instance.RunData; }
        }

        private CurrentWeatherResult _CurrentWeather;
        public CurrentWeatherResult CurrentWeather
        {
            get { return _CurrentWeather; }
            set
            {
                _CurrentWeather = value;
                RaisePropertyChanged("CurrentWeather");
            }
        }

        public int _Heartrate;
        public int Heartrate { get { return _Heartrate; } private set { Set(ref _Heartrate, value); } }

        public MapRoute _MyRoute;
        public MapRoute MyRoute { get { return _MyRoute; } private set { Set(ref _MyRoute, value); } }

        public Geopoint _CurrentLocation;
        public Geopoint CurrentLocation { get { return _CurrentLocation; }
            set { Set(ref _CurrentLocation, value); } }

        public bool WaitingForWheather { get; set; }




        #endregion

        public SoloRunViewModel() : base()
        {
            StartRun = new RelayCommand(() => {
                    RunManager.Instance.Start();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    RunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
                    RunManager.Instance.OnVoiceMsgUpdate += Instance_OnMsgUpdate;
            }, 
                () => { return (!RunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
                {
                    RunManager.Instance.Stop();  
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    RunManager.Instance.OnRouteUpdate -= Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                    RunManager.Instance.OnVoiceMsgUpdate -= Instance_OnMsgUpdate;
                },
                () => { return RunManager.Instance.InRun; });

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
            speechEvent = new ManualResetEvent(true);
            WaitingForWheather = false;



        }

        public void Instance_OnHeartRateChange(object sender, int e)
        {
            Heartrate = e;
        }

        public void Instance_OnLocationChange(Geoposition obj)
        {
            CurrentLocation = obj.ToGeoPoint();

            CurrentWeather = WeatherService.Instance.GetWeatherForLocation(obj.Coordinate.Point.Position.Longitude,
                obj.Coordinate.Point.Position.Latitude).Result;
        }

        public void Instance_OnRouteUpdate(object sender, MapRoute e)
        {
            MyRoute = e;
        }

        private async void Instance_OnMsgUpdate(string obj)
        {
            await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await RunManager.Instance.ReadText(obj);
            });

        }
    }
}

