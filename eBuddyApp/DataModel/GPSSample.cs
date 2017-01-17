using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace eBuddy
{
    public class GPSSample
    {
        public string Id { get; set; }

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

        public static GPSSample FromGeoposition(Geoposition pos, DateTime time)
        {
            if (pos == null)
                return null;

            return new GPSSample()
            {
                Accuracy = pos.Coordinate.Accuracy,
                Longitude = pos.Coordinate.Longitude,
                Latitude = pos.Coordinate.Latitude,
                Altitude = pos.Coordinate.Altitude.HasValue ? pos.Coordinate.Altitude.Value : -1,
                Time = time
            };
        }
    }
}
