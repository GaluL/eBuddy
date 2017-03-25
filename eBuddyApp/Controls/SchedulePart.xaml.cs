using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddyApp;
using eBuddyApp.Services.Azure;
using eBuddyApp.Views;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class SchedulePart : UserControl
    {
        public DateTime Date { get { return datePicker.Date.Date + timePicker.Time; } }
        public double Distance { get { return distanceTextBox.Text.Length != 0 ? double.Parse(distanceTextBox.Text) : 5; } }

        public SchedulePart()
        {
            this.InitializeComponent();
        }

        public event EventHandler HideRequested;
        public event EventHandler InviteRequested;

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, null);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            InviteRequested?.Invoke(this, null);
        }
    }
}
