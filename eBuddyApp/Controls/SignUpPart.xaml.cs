using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddyApp.Models;
using eBuddyApp.Services.Azure;
using eBuddyApp.Views;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class SignUpPart : UserControl
    {
        private UserItem UserData;

        public SignUpPart()
        {
            this.InitializeComponent();

            UserData = new UserItem();
        }

        public event EventHandler SignUpHideRequested;
        public event EventHandler SignedUp;


        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            if (await MobileService.Instance.FacebookLogIn())
            {
                SignedUp?.Invoke(this, EventArgs.Empty);
                SignUpHideRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            SignUpHideRequested?.Invoke(this, EventArgs.Empty);
        }

        //public Models.UserCredentials UserCredentials { get; set; }
        //private void fbSignup_OnClick(object sender, RoutedEventArgs e)
        //{
        //    SignUpRequested?.Invoke(this, null);
        //}
        private async void SignUpbtn_Click(object sender, RoutedEventArgs e)
        {
            Busy.SetBusy(true, "Registering...");

            if (await MobileService.Instance.AuthenticateWithFacebook())
            {
                MobileService.Instance.RegisterUser(UserData);
                SignedUp?.Invoke(this, EventArgs.Empty);
            }

            Busy.SetBusy(false);
            SignUpHideRequested?.Invoke(this, EventArgs.Empty);


        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
