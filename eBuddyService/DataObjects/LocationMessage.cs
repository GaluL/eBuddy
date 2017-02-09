using System;
using Newtonsoft.Json;

namespace eBuddyService.DataObjects
{
    public class LocationMessage
    {
        [JsonProperty(PropertyName = "sourceUserId")]
        public string SourceUserId;

        [JsonProperty(PropertyName = "destUserId")]
        public string DestUserId;

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude;

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude;

        [JsonProperty(PropertyName = "altitude")]
        public double Altitude;

        [JsonProperty(PropertyName = "accuracy")]
        public double Accuracy;

        [JsonProperty(PropertyName = "time")]
        public DateTime Time;
    }
}
