using System.ComponentModel;
using System.Linq;
using System;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddy;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace eBuddyApp.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        Services.SettingsServices.SettingsService _settings;

        public RelayCommand PairBand;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = Services.SettingsServices.SettingsService.Instance;

            // Add checking if already logged in
            LoginModal.IsModal = true;
            SignUpModal.IsModal = false;

            PairBand = new RelayCommand(() => BandModal.IsModal = true, () => { return !BandService.Instance.IsConnected; } );
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);

            BuddyRunManager.Instance.RunAboutToStart += Instance_RunAboutToStart; 
        }

        private async void Instance_RunAboutToStart(object sender, EventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                HamburgerMenu.NavigationService.Navigate(typeof(SocialRunPage));
            });
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Login

        private void LoginTapped(object sender, RoutedEventArgs e)
        {
            LoginModal.IsModal = true;
        }

        private void LoginHide(object sender, System.EventArgs e)
        {
            //LoginButton.IsEnabled = true;
            LoginModal.IsModal = false;
        }

        private void LoginLoggedIn(object sender, EventArgs e)
        {
            //LoginButton.IsEnabled = false;
            LoginModal.IsModal = false;
        }
    
        private void LoginSignUp(object sender, EventArgs e)
        {
            LoginModal.IsModal = false;
            SignUpModal.IsModal = true;
        }
        #endregion

        #region SignUp
        private void signUpHide(object sender, System.EventArgs e)
        {
            //LoginButton.IsEnabled = true;
            SignUpModal.IsModal = false;
        }
        private void signedUp(object sender, System.EventArgs e)
        {
            //LoginButton.IsEnabled = true;
            SignUpModal.IsModal = false;
        }

        #endregion

        #region Band

        private void HamburgerButtonInfo_OnTapped(object sender, RoutedEventArgs e)
        {
            BandModal.IsModal = true;
        }

        private void BandPart_OnHideRequested(object sender, EventArgs e)
        {
            BandModal.IsModal = false;

            PairBand.RaiseCanExecuteChanged();
        }

        private void Summary_OnHideRequested(object sender, EventArgs e)
        {
            SummaryRunModal.IsModal = false;
        }

        private void Summary(object sender, System.EventArgs e)
        {
            SummaryRunModal.IsModal = true;
        }

        private void ScoreSummary_OnHideRequested(object sender, EventArgs e)
        {
            ScoreSummaryRunModal.IsModal = false;
        }

        private void ScoreSummary(object sender, System.EventArgs e)
        {
            ScoreSummaryRunModal.IsModal = true;
        }
        #endregion
    }
}

