using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using eBuddyApp.Models;
using eBuddyApp.Services.Azure;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;

namespace eBuddyApp.ViewModels
{
    public class SuggestionsPageViewModel : ViewModelBase
    {
        internal RelayCommand Update;
        internal List<UserItem> Suggestions
        {
            get { return MobileService.Instance.Suggestions; }
        }

        public SuggestionsPageViewModel()
        {
            Update = new RelayCommand(async () =>
            {
                await MobileService.Instance.GetSuggestions();
                RaisePropertyChanged("Suggestions");
            });
        }
    }
}

