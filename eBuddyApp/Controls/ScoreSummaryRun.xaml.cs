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
    public sealed partial class ScoreSummaryRun : UserControl
    {

        


        public ScoreSummaryRun()
        {

            this.InitializeComponent();
           ScoreRunManager.scoreRunInstance.OnPopped += pop_Click;
        }

        public event EventHandler HideRequested;
        
        public event EventHandler SummaryPop;



        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }
        public  async void pop_Click(object c)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SummaryPop?.Invoke(this, EventArgs.Empty);
            });
        }

    
    }
}
