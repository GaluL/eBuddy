using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Band.Sensors;
using Newtonsoft.Json;

namespace eBuddy
{
    public class HeartRateSample
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "heartrate")]
        public double HeartRate;

        [JsonProperty(PropertyName = "quality")]
        public HeartRateQuality Quality;

        [JsonProperty(PropertyName = "time")]
        public DateTime Time;

        public HeartRateSample(int sensorReadingHeartRate, HeartRateQuality sensorReadingQuality, DateTimeOffset sensorReadingTimestamp)
        {
            HeartRate = sensorReadingHeartRate;
            Quality = sensorReadingQuality;
            Time = sensorReadingTimestamp.UtcDateTime;
        }
    }
}
