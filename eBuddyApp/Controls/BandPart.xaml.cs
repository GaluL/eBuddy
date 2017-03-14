using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddy;
using eBuddyApp.Services.Azure;
using eBuddyApp.Views;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class BandPart : UserControl
    {
        public BandPart()
        {
            this.InitializeComponent();
        }

        public event EventHandler HideRequested;
        public event EventHandler LoggedIn;

        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            Busy.SetBusy(true, "Pairing...");

            if (await BandService.Instance.Connect())
            {
                LoggedIn?.Invoke(this, null);
                HideRequested?.Invoke(this, null);
            }

            Busy.SetBusy(false);
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
