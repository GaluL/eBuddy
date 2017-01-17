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
    public sealed partial class BandPage : Page
    {
        public BandPage()
        {
            this.InitializeComponent();
            BandHandler.Instance.OnConnectionStatusChange += Instance_OnConnectionStatusChange;
        }

        private void Instance_OnConnectionStatusChange(bool obj)
        {
            if (obj)
            {
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            BandHandler.Instance.Connect();
        }
    }
}
