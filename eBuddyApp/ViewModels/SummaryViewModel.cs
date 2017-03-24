using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Azure;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;

namespace eBuddyApp.ViewModels
{
    public class SummaryViewModel : ViewModelBase
    {

        public DateTime Date
        {
            get { return RunManager.Instance.RunData.Date; }
        }

        public double Distance
        {
            get { return RunManager.Instance.RunData.Distance; }
        }

        public double Speed
        {
            get { return RunManager.Instance.RunData.Speed; }
        }

        public TimeSpan Time
        {
            get { return RunManager.Instance.RunData.Time; }
        }

        public int MaxHeartRate
        {
            get { return RunManager.Instance.MaxHeartRate; }
        }

        public double Score
        {
            get { return MobileService.Instance.UserData.Score; }
        }



        public SummaryViewModel()
        {

        }
    }


    //    public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
    //    {
    //        WelcomeText = (suspensionState.ContainsKey(nameof(WelcomeText))) ? suspensionState[nameof(WelcomeText)]?.ToString() : parameter?.ToString();
    //        await Task.CompletedTask;
    //    }

    //    public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
    //    {
    //        if (suspending)
    //        {
    //            suspensionState[nameof(WelcomeText)] = WelcomeText;
    //        }
    //        await Task.CompletedTask;
    //    }

    //    public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
    //    {
    //        args.Cancel = false;
    //        await Task.CompletedTask;
    //    }
    }



