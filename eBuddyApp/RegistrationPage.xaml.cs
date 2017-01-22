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
using eBuddy.DataModel;
using Microsoft.WindowsAzure.MobileServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace eBuddy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegistrationPage : Page
    {
        IMobileServiceTable<UserItem> usersTable = App.MobileService.GetTable<UserItem>();

        public RegistrationPage()
        {
            this.InitializeComponent();

            profileControl.ViewModel = UserDataProvider.ViewModel;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserDataProvider.RegisterUser();

            this.Frame.Navigate(typeof(BandPage));
        }
    }
}
