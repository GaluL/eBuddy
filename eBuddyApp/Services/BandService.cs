using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eBuddyApp.Services.Azure;
using Microsoft.Band;

namespace eBuddy
{
    class BandService
    {
        private static BandService _instance;
        public static BandService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BandService();
                }

                return _instance;
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                OnConnectionStatusChange?.Invoke(value);
            }
        }

        private int _heartRate;
        private static int TEN = 0;
        private int avg_heartRate = 0;


        public int HeartRate
        {
            get { return _heartRate; }
            private set
            {
                _heartRate = value;
                OnHeartRateChange?.Invoke(this, value);
            }
        }

        public int RestHeartRate { get; private set; }
        public int MinTargetZoneHeartRate { get; private set; }
        public int MaxTargetZoneHeartRate { get; private set; }



        public event Action<bool> OnConnectionStatusChange;
        public event EventHandler<int> OnHeartRateChange;

        private BandService()
        {
            MinTargetZoneHeartRate = 161;
            MaxTargetZoneHeartRate = 200;
        }

        public async Task<bool> Connect()
        {
            try
            {
                OnHeartRateChange += CalculateRestHeartRate;

                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    //this.viewModel.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return false;
                }

                // Connect to Microsoft Band.
                IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

                bool heartRateConsentGranted;

                // Check whether the user has granted access to the HeartRate sensor.
                if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() == UserConsent.Granted)
                {
                    heartRateConsentGranted = true;
                }
                else
                {
                    heartRateConsentGranted = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                }

                if (!heartRateConsentGranted)
                {
                    //this.viewModel.StatusMessage = "Access to the heart rate sensor is denied.";
                }
                else
                {
                    // Subscribe to HeartRate data.
                    bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) =>
                    {
                        HeartRate = args.SensorReading.HeartRate;
                    };
                }
                ;
                await bandClient.SensorManager.HeartRate.StartReadingsAsync();

                IsConnected = true;

                return true;
            }
            catch (Exception ex)
            {
                return false;
                //this.viewModel.StatusMessage = ex.ToString();
            }
        }

        private void CalculateTargetZoneHeartRate()
        {
            int maxHeartRate = 220 - (int) MobileService.Instance.UserData.Age;
            int HHR = maxHeartRate - RestHeartRate;
            MinTargetZoneHeartRate = (int) (HHR * (0.7) + RestHeartRate);
            MaxTargetZoneHeartRate = (int) (HHR * 0.85 + RestHeartRate);
        }


        public void CalculateRestHeartRate(object sender, int e)
        {
            avg_heartRate += e;
            TEN++;
            if (TEN >= 10)
            {
                /*wait for 10 heartRate change events*/
                RestHeartRate = avg_heartRate / TEN;
                OnHeartRateChange -= CalculateRestHeartRate; //no need
                CalculateTargetZoneHeartRate();
                avg_heartRate = 0; // reset
                TEN = 0; // reset
            }
        }
    }
}
