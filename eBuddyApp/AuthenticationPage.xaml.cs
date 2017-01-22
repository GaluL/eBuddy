using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace eBuddy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthenticationPage : Page
    {
        public AuthenticationPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonFbLogin_Click(object sender, RoutedEventArgs e)
        {
            // Login the user and then load data from the mobile app.
            if (await Authenticator.Instance.AuthenticateWithFacebook())
            {
                if (await UserDataProvider.InitModel())
                {
                    this.Frame.Navigate(typeof(BandPage));
                }
                else
                {
                    this.Frame.Navigate(typeof(RegistrationPage));
                }
            }
        }

        private void ButtonGoogleLogin_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {

            throw new NotImplementedException();
        }

        private void ButtomServerPing_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
