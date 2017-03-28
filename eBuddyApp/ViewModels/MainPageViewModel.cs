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
    public class MainPageViewModel : ViewModelBase
    {
        private string _WelcomeText = "Welcome Back!";
        public string WelcomeText { get { return _WelcomeText; } set { Set(ref _WelcomeText, value); } }

        public string UserName
        {
            get { return MobileService.Instance.UserData.PrivateName; }
        }

        internal ObservableCollection<RunItem> _FinishedRuns;
        internal ObservableCollection<RunItem> FinishedRuns { get { return _FinishedRuns; } set { Set(ref _FinishedRuns, value); } }

        internal ObservableCollection<ScheduledRunItem> _UpcomingRuns;
        internal ObservableCollection<ScheduledRunItem> UpcomingRuns { get { return _UpcomingRuns; } set { Set(ref _UpcomingRuns, value); } }

        internal RelayCommand Update;

        public MainPageViewModel()
        {
            Update = new RelayCommand(async () =>
            {
                await MobileService.Instance.CollectScheduledRuns();
                RaisePropertyChanged("UpcomingRuns");

                await MobileService.Instance.CollectFinishedRuns();
                RaisePropertyChanged("FinishedRuns");
            });

            MobileService.Instance.UserDataLoaded += Instance_UserDataLoaded;
        }

        private void Instance_UserDataLoaded(object sender, EventArgs e)
        {
            WelcomeText = String.Format("Welcome back {0}!", MobileService.Instance.UserData.PrivateName);
            FinishedRuns = MobileService.Instance.FinishedRuns;
            UpcomingRuns = MobileService.Instance.ScheduledRuns;
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
}

