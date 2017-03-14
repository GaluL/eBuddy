using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Template10.Mvvm;

namespace eBuddyApp.Models
{
    public class RunItem : BindableBase
    {
        public  string Id { get; set; }

        string _FacebookId = default(string);
        [JsonProperty(PropertyName = "facebookid")]
        public string FacebookId { get { return _FacebookId; } set { Set(ref _FacebookId, value); } }

        DateTime _Date = default(DateTime);
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get { return _Date; } set { Set(ref _Date, value); } }

        TimeSpan _Time = default(TimeSpan);
        [JsonProperty(PropertyName = "time")]
        public TimeSpan Time { get { return _Time; } set { Set(ref _Time, value); } }

        double _Distance = default(double);
        [JsonProperty(PropertyName = "distance")]
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        double _Speed = default(double);
        [JsonProperty(PropertyName = "speed")]
        public double Speed { get { return _Speed; }
            set { Set(ref _Speed, value); } }
    }
}
