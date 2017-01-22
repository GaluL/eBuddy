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
using eBuddy.ViewModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace eBuddy
{
    public sealed partial class ProfileUC : UserControl
    {
        internal UserViewModel ViewModel
        { get; set; }
        public ProfileUC()
        {
            this.InitializeComponent();

            ViewModel = new UserViewModel();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PrivateName":
                {
                    privateNameTextbox.Text = ViewModel.PrivateName;
                    break;
                }
                case "SurName":
                {
                    surNameTextbox.Text = ViewModel.SurName;
                    break;
                }
                case "Age":
                {
                    ageTextbox.Text = ViewModel.Age;
                    break;
                }
                case "Weight":
                {
                    weightTextbox.Text = ViewModel.Weight;
                    break;
                }
                case "Height":
                {
                    heightTextbox.Text = ViewModel.Height;
                    break;
                }
            }
        }

        private void PrivateNameTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.PrivateName = privateNameTextbox.Text;
        }

        private void SurNameTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.SurName = surNameTextbox.Text;
        }

        private void AgeTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.Age = ageTextbox.Text;
        }

        private void WeightTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.Weight = weightTextbox.Text;
        }

        private void HeightTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.Height = heightTextbox.Text;
        }
   
        internal UserItem GetProfile()
        {
            return ViewModel.Model;
        }
    }
}
