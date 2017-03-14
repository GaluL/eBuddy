using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private String _User1FacebookId = default(string);
        [JsonProperty(PropertyName = "user1facebookid")]
        public String User1FacebookId { get { return _User1FacebookId; } set { Set(ref _User1FacebookId, value); } }

        private String _User2FacebookId = default(string);
        [JsonProperty(PropertyName = "user2facebookid")]
        public String User2FacebookId { get { return _User2FacebookId; } set { Set(ref _User2FacebookId, value); } }

        private bool _User1Approved = default(bool);
        [JsonProperty(PropertyName = "user1approved")]
        public bool User1Approved { get { return _User1Approved; } set { Set(ref _User1Approved, value); } }

        private bool _User2Approved = default(bool);
        [JsonProperty(PropertyName = "user2approved")]
        public bool User2Approved { get { return _User2Approved; } set { Set(ref _User2Approved, value); } }

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
    }
}
