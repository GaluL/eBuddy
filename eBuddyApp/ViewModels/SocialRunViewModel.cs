using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace eBuddyApp.ViewModels
{
    class SocialRunViewModel : SoloRunViewModel
    {

        private MapRoute _BuddyRoute;
        public MapRoute BuddyRoute { get { return _BuddyRoute; } private set { Set(ref _BuddyRoute, value); } }

        private Geopoint _BuddyLocation;
        public Geopoint BuddyLocation { get { return _BuddyLocation; } private set { Set(ref _BuddyLocation, value); } }

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

        public SocialRunViewModel()
        {
            SocialMsg = "Let's start running! press start as soon as you are ready";
            SocialColor = Colors.DarkGray;
            SocialSize = 18;
            StartRun = new RelayCommand(() =>
            {
                BuddyRunManager.Instance.Start();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
            },
            () => { return (!BuddyRunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
            {
                BuddyRunManager.Instance.Stop();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
            },
                () => { return BuddyRunManager.Instance.InRun; });

            BuddyRunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
            CurrentLocation = ExtentionMethods.GetDefaultPoint();
            BuddyLocation = ExtentionMethods.GetDefaultPoint();
            BuddyRunManager.Instance.OnBuddyRouteUpdate += Instance_OnBuddyRouteUpdate;
            BuddyRunManager.Instance.OnBuddyLocationUpdate += Instance_OnBuddyLocationUpdate;
            BuddyRunManager.Instance.OnMsgUpdate += Instance_OnMsgUpdate;
            BuddyRunManager.Instance.OnMsgColorUpdate += Instance_OnMsgColorUpdate;
            BuddyRunManager.Instance.OnMsgSizeUpdate += Instance_OnMsgSizeUpdate;


        }

        private void Instance_OnMsgSizeUpdate(int obj)
        {
            SocialSize = obj;
        }

        private void Instance_OnMsgColorUpdate(Color obj)
        {
            SocialColor = obj;
        }

        private void Instance_OnMsgUpdate(string obj)
        {
            SocialMsg = obj;

        }

        private void Instance_OnBuddyRouteUpdate(MapRoute e)
        {
            BuddyRoute = e;
        }

        private void Instance_OnBuddyLocationUpdate(Geopoint obj)
        {
            BuddyLocation = obj;
        }
    }





}
