using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddyApp;
using eBuddyApp.Services.Azure;
using eBuddyApp.Views;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class LoginPart : UserControl
    {
        public LoginPart()
        {
            this.InitializeComponent();
        }

        public event EventHandler HideRequested;
        public event EventHandler LoggedIn;
        public event EventHandler SignUpRequested;

        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            Busy.SetBusy(true, "Connecting...");

            if (await MobileService.Instance.FacebookLogIn())
            {
                if (await MobileService.Instance.CheckRegistation())
                {
                    LoggedIn?.Invoke(this, EventArgs.Empty);
                    HideRequested?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    SignUpRequested?.Invoke(this, null);
                }
            }
            Busy.SetBusy(false);
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }

        //public Models.UserCredentials UserCredentials { get; set; }
        private void fbSignup_OnClick(object sender, RoutedEventArgs e)
        {
            SignUpRequested?.Invoke(this, null);             
        }
    }
}
