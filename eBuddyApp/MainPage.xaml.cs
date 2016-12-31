/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Security.Credentials;
using eBuddy.DataModel;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync
#endif

namespace eBuddy
{
    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<TodoItem, TodoItem> items;
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<TodoItem> todoTable = App.MobileService.GetSyncTable<TodoItem>(); // offline sync
#else
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();
#endif
        private MobileServiceCollection<eBuddyUser1, eBuddyUser1> Users;

        private IMobileServiceTable<eBuddyUser1> eBuddyUserTable = App.MobileService.GetTable<eBuddyUser1>();



        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loading;
        }

        public static readonly Geopoint SeattleGeopoint = new Geopoint(new BasicGeoposition() { Latitude = 32.08, Longitude = 34.77 });


        private void MainPage_Loading(object sender, RoutedEventArgs e)
        {
            if (!Authenticator.Instance.IsAuthenticated)
            {
                this.Frame.Navigate(typeof(AuthenticationPage));
            }

            //if (!BandHandler.Instance.IsPaired)
            //{
            //    this.Frame.Navigate(typeof(BandPage));
            //}
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync(); // offline sync
#endif
            //ButtonRefresh_Click(this, null);
        }

        private async Task InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. After the operation completes
            // and the mobile app backend has assigned an id, the item is added to the CollectionView.
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);
            

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async Task InsertUserToDataBase(eBuddyUser1 eBuddyUser)
        {
            // This code inserts a new TodoItem into the database. After the operation completes
            // and the mobile app backend has assigned an id, the item is added to the CollectionView.
            await eBuddyUserTable.InsertAsync(eBuddyUser);
            Users.Add(eBuddyUser);


#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var eBuddyuser = new eBuddyUser1 { Id = "3030", name = "ofek", age = 10, height = 175, weight = 70};
            await InsertUserToDataBase(eBuddyuser);
        }


        //private async Task RefreshTodoItems()
        //{
        //    MobileServiceInvalidOperationException exception = null;
        //    try
        //    {
        //        // This code refreshes the entries in the list view by querying the TodoItems table.
        //        // The query excludes completed TodoItems.
        //        items = await todoTable
        //            .Where(todoItem => todoItem.Complete == false)
        //            .ToCollectionAsync();
        //    }
        //    catch (MobileServiceInvalidOperationException e)
        //    {
        //        exception = e;
        //    }

        //    if (exception != null)
        //    {
        //        await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
        //    }
        //    else
        //    {
        //        ListItems.ItemsSource = items;
        //        this.ButtonSave.IsEnabled = true;
        //    }
        //}

        //        private async Task UpdateCheckedTodoItem(TodoItem item)
        //        {
        //            // This code takes a freshly completed TodoItem and updates the database.
        //			// After the MobileService client responds, the item is removed from the list.
        //            await todoTable.UpdateAsync(item);
        //            items.Remove(item);
        //            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

        //#if OFFLINE_SYNC_ENABLED
        //            await App.MobileService.SyncContext.PushAsync(); // offline sync
        //#endif
        //        }

        //        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        //        {
        //            ButtonRefresh.IsEnabled = false;

        //#if OFFLINE_SYNC_ENABLED
        //            await SyncAsync(); // offline sync
        //#endif
        //            await RefreshTodoItems();

        //            ButtonRefresh.IsEnabled = true;
        //        }

        //private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{
        //    var todoItem = new TodoItem { Text = TextInput.Text };
        //    TextInput.Text = "";
        //    await InsertTodoItem(todoItem);
        //}

        //private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox cb = (CheckBox)sender;
        //    TodoItem item = cb.DataContext as TodoItem;
        //    await UpdateCheckedTodoItem(item);
        //}

        //private void TextInput_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter) {
        //        ButtonSave.Focus(FocusState.Programmatic);
        //    }
        //}

        #region Offline sync
#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
           if (!App.MobileService.SyncContext.IsInitialized)
           {
               var store = new MobileServiceSQLiteStore("localstore.db");
               store.DefineTable<TodoItem>();
               await App.MobileService.SyncContext.InitializeAsync(store);
           }

           await SyncAsync();
        }

        private async Task SyncAsync()
        {
           await App.MobileService.SyncContext.PushAsync();
           await todoTable.PullAsync("todoItems", todoTable.CreateQuery());
        }
#endif
        #endregion
    }
}
