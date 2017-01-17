using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Band;

namespace eBuddy
{
    class BandHandler
    {
        private static BandHandler _instance;
        public static BandHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BandHandler();
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

        private HeartRateSample _heartRate;
        public HeartRateSample HeartRate
        {
            get { return _heartRate; }
            private set
            {
                _heartRate = value;
                OnHeartRateChange?.Invoke(this, value);
            }
        }

        public event Action<bool> OnConnectionStatusChange;
        public event EventHandler<HeartRateSample> OnHeartRateChange;

        private BandHandler()
        {
        }

        public async Task Connect()
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    //this.viewModel.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
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
                    bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => {
                        HeartRate = new HeartRateSample(args.SensorReading.HeartRate, args.SensorReading.Quality, args.SensorReading.Timestamp);
                    };
                }
                ;
                await bandClient.SensorManager.HeartRate.StartReadingsAsync();

                IsConnected = true;
            }
            catch (Exception ex)
            {
                //this.viewModel.StatusMessage = ex.ToString();
            }
        }
    }
}
