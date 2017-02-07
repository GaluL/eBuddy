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
using eBuddy.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace eBuddy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MeTab : Page
    {
        private UserViewModel _userViewModel;

        internal UserViewModel UserViewModel
        {
            get { return _userViewModel; }
            set
            {
                _userViewModel = value;

                ShowUserData();
            }
        }

        public MeTab()
        {
            this.InitializeComponent();
        }

        private void ShowUserData()
        {
            helloTextblock.Text = "Hi " + UserDataProvider.ViewModel.PrivateName + " " + UserDataProvider.ViewModel.SurName +
                      "! Nice to see you again.";

            scoreTextblock.Text = "Your current score is: "; //+ UserDataProvider.ViewModel.Model.MAS;
        }
    }
}
