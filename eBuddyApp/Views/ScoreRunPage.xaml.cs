﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace eBuddyApp.Views
{
    public sealed partial class ScoreRunPage : Page
   {
       public ScoreRunPage()
        {
            this.InitializeComponent();
            ViewModel.page = this;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("MyRoute"))
            {
                myMap.Routes.Clear();
                myMap.Routes.Add(new MapRouteView(ViewModel.MyRoute));
            }

            if (e.PropertyName.Equals("CurrentWeather"))
            {

                if (ViewModel.CurrentWeather != null)
                {
                    if (ViewModel.CurrentWeather.Icon != null && !ViewModel.CurrentWeather.Icon.Equals(String.Empty))
                    {
                        weatherIcon.Source =
                            new BitmapImage(
                                new Uri(
                                    String.Format(@"http://openweathermap.org/img/w/{0}.png",
                                        ViewModel.CurrentWeather.Icon), UriKind.Absolute));
                    }
                }
            }
        }
    }
}

