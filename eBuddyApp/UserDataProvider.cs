using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBuddy.DataModel;
using eBuddy.ViewModel;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace eBuddy
{
    class UserDataProvider
    {
        private static UserDataProvider _instance;
        private static UserItem _model = null;
        private static UserViewModel _viewModel = null;
        private static IMobileServiceTable<UserItem> usersTable;
        private static bool _initialized = false;

        public static UserViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public static bool Initialized
        {
            get { return _initialized; }
        }

        internal static async Task<bool> InitModel()
        {
            bool res = true;

            usersTable = App.MobileService.GetTable<UserItem>();
            var users = await usersTable.ToCollectionAsync();

            try
            {
                _model = users.First(x => x.FacebookId == App.MobileService.CurrentUser.UserId);
            }
            catch (Exception e)
            {
                _model = new UserItem();
                res = false;
            }

            _viewModel = new UserViewModel(_model);

            _initialized = true;

            return res;
        }

        internal static async void RegisterUser()
        {
            _model.FacebookId = App.MobileService.CurrentUser.UserId;

            await usersTable.InsertAsync(_model);
        }
    }
}
