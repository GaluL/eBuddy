using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.NetworkOperators;
using eBuddyApp.Services.Azure;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Template10.Mvvm;

namespace eBuddyApp.Models
{
    enum EScheduleStauts
    {
        Scheduled,
        PendingMyApproval,
        PendingBuddyApproval    
    }

    class ScheduledRunItem : BindableBase
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "InitializerFacebookId")]
        public String InitializerFacebookId { get; set; }
        [JsonProperty(PropertyName = "BuddyFacebookId")]
        public String BuddyFacebookId { get; set; }
        [JsonProperty(PropertyName = "BuddyApproval")]
        public bool BuddyApproval { get; set; }

        private DateTime _Date = default(DateTime);
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get { return _Date; } set { Set(ref _Date, value); } }

        private double _Distance = default(double);
        [JsonProperty(PropertyName = "distance")]
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        private bool _Finished = default(bool);
        [JsonProperty(PropertyName = "finished")]
        public bool Finished { get { return _Finished; } set { Set(ref _Finished, value); } }

        private string _Winner = default(string);
        [JsonProperty(PropertyName = "winner")]
        public String Winner { get { return _Winner; } set { Set(ref _Winner, value); } }

        [JsonIgnore]
        public bool WaitingForMyApproval { get; set; }

        [JsonIgnore]
        public RelayCommand Approve { get; set; }

        public ScheduledRunItem()
        {
            Approve = new RelayCommand(async () =>
            {
                BuddyApproval = true;
                await MobileService.Instance.Service.GetTable<ScheduledRunItem>().UpdateAsync(this);
                await MobileService.Instance.CollectScheduledRuns();
            });
        }
    }
}
