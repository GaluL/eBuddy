

using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace eBuddyApp.Models
{
    public class BuddyRunInfo
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

        public static BuddyRunInfo FromGeoposition(Geoposition pos, DateTime time)
        {
            if (pos == null)
                return null;

            return new BuddyRunInfo()
            {
                Accuracy = pos.Coordinate.Accuracy,
                Longitude = pos.Coordinate.Longitude,
                Latitude = pos.Coordinate.Latitude,
                Altitude = pos.Coordinate.Altitude.HasValue ? pos.Coordinate.Altitude.Value : -1,
                Time = time
            };
        }

        public Geopoint GetGeoPoint()
        {
            return new Geopoint(new BasicGeoposition()
            {
                Altitude = this.Altitude,
                Latitude = this.Latitude,
                Longitude = this.Longitude
            });
        }
    }
}
