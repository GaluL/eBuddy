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
    public class ScoreSummaryViewModel : ViewModelBase
    {

        private double _MAS;
        public Double MAS
        {
            get { return _MAS; }
            set { _MAS = value; }
        }

        private double _Vo2Max;
        public double Vo2Max
        {
            get { return _Vo2Max; }
            set { _Vo2Max = value; }
        }

        private double _Score;
        public double Score
        {
            get { return _Score; }
            set { _Score = value; }
        }



        public ScoreSummaryViewModel()
        {
            ScoreRunManager.scoreRunInstance.OnIsFinishedChange += ScoreRunInstance_OnIsFinishedChange;
        }


        private void ScoreRunInstance_OnIsFinishedChange(object sender, bool e)
        {
            if (e)
            {
                Score = MobileService.Instance.UserData.Score;
                Vo2Max = ScoreRunManager.scoreRunInstance.Vo2Max;
                MAS = ScoreRunManager.scoreRunInstance.MAS;
            }
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



