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
    class ScoreRunViewModel : SoloRunViewModel
    {
        public override RunItem RunData { get { return ScoreRunManager.Instance.RunData; } }

        public ScoreRunViewModel()
        {
            StartRun = new RelayCommand(() => {
                ScoreRunManager.Instance.Start();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
            },
            () => { return (!RunManager.Instance.InRun && BandService.Instance.IsConnected); });

            StopRun = new RelayCommand(() =>
            {
                ScoreRunManager.Instance.Stop();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
            },
            () => { return RunManager.Instance.InRun; });

            BandService.Instance.OnConnectionStatusChange += Instance_OnConnectionStatusChange;
        }

        private void Instance_OnConnectionStatusChange(bool obj)
        {
            StartRun.RaiseCanExecuteChanged();
        }
    }
}
