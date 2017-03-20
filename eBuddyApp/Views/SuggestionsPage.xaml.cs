using System;
using Windows.UI.Xaml;
using eBuddyApp.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using eBuddyApp.Models;
using eBuddyApp.Services.Azure;
using Template10.Samples.SearchSample.Controls;

namespace eBuddyApp.Views
{
    public sealed partial class SuggestionsPage : Page
    {
        public SuggestionsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //var popup = new SchedulePart(((UserItem)suggestionsFlipView.SelectedItem).FacebookId);

            //await popup.ShowAsync();

            scheduleModal.IsModal = true;
        }

        private void SchedulePart_OnHideRequested(object sender, EventArgs e)
        {
            scheduleModal.IsModal = false;
        }

        private void SchedulePart_OnInviteRequested(object sender, EventArgs e)
        {
            scheduleModal.IsModal = false;

            MobileService.Instance.ScheduleARun(((UserItem) suggestionsFlipView.SelectedItem).FacebookId,
                schedulePart.Distance, schedulePart.Date);
        }
    }
}

