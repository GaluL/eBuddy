using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace eBuddyApp.ViewModels
{
    class SoloRunViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;

        #endregion

        #region Properties

        public bool IsScoreRun { get; set; }

        public virtual RunItem RunData
        {
            get { return RunManager.Instance.RunData; }
        }

        public int _Heartrate;
        public int Heartrate { get { return _Heartrate; } private set { Set(ref _Heartrate, value); } }

        public MapRoute _MyRoute;
        public MapRoute MyRoute { get { return _MyRoute; } private set { Set(ref _MyRoute, value); } }

        public Geopoint _CurrentLocation;
        public Geopoint CurrentLocation { get { return _CurrentLocation; }
            set { Set(ref _CurrentLocation, value); } }

        #endregion

        public SoloRunViewModel() : base()
        {
            StartRun = new RelayCommand(() => {
                    RunManager.Instance.Start();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                }, 
                () => { return (!RunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
                {
                    RunManager.Instance.Stop();  
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                },
                () => { return RunManager.Instance.InRun; });

            RunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
        }

        public void Instance_OnHeartRateChange(object sender, int e)
        {
            Heartrate = e;
        }

        public void Instance_OnLocationChange(Geoposition obj)
        {
            CurrentLocation = obj.ToGeoPoint();
        }

        public void Instance_OnRouteUpdate(object sender, MapRoute e)
        {
            MyRoute = e;
        }
    }
}
