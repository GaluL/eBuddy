using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddy;
using eBuddyApp;
using eBuddyApp.Services.Azure;
using eBuddyApp.Views;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class SummaryRun : UserControl
    {

        public event EventHandler SummaryPop;


        public SummaryRun()
        {

            this.InitializeComponent();
           RunManager.Instance.OnPopped += pop_Click;
        }

        public event EventHandler HideRequested;

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }
        public void pop_Click(object c)
        {
            SummaryPop?.Invoke(this, EventArgs.Empty);
        }

    
    }
}
