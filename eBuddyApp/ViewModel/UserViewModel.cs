using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using eBuddy.DataModel;

namespace eBuddy.ViewModel
{
    class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UserItem Model { get; set; }

        public String PrivateName
        {
            get { return Model.PrivateName; }
            set
            {
                Model.PrivateName = value;
                OnPropertyChanged("PrivateName");
            }
        }

        public String SurName
        {
            get { return Model.SurName; }
            set
            {
                Model.SurName = value;
                OnPropertyChanged("SurName");
            }
        }

        public String Age
        {
            get { return Model.Age.ToString(); }
            set
            {
                double res;

                if (double.TryParse(value, out res))
                {
                    Model.Age = res;
                }

                OnPropertyChanged("Age");
            }
        }

        public String Weight
        {
            get { return Model.Weight.ToString(); }
            set
            {
                double res;

                if (double.TryParse(value, out res))
                {
                    Model.Weight = res;
                }

                OnPropertyChanged("Weight");
            }
        }

        public String Height
        {
            get { return Model.Height.ToString(); }
            set
            {
                double res;

                if (double.TryParse(value, out res))
                {
                    Model.Height = res;
                }

                OnPropertyChanged("Height");
            }
        }

        public UserViewModel() : this (new UserItem()) {}

        public UserViewModel(UserItem user)
        {
            Model = user;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
