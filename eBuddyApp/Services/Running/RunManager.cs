using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Microsoft.WindowsAzure.MobileServices;
using Template10.Common;

namespace eBuddy
{
    class RunManager
    {
        private int _lastLocationTimeSeconds = 1;
        private static RunManager _Instance;
        public static RunManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new RunManager();
                }

                return _Instance;
            }
        }


        private string _voiceMsg;
        public string VoiceMsg
        {
            get { return _voiceMsg; }
            set
            {
                _voiceMsg = value;
                OnVoiceMsgUpdate?.Invoke(value);
            }
        }

        private RunItem _RunData;
        public RunItem RunData
        {
            get { return _RunData; }
            private set { _RunData = value; }
        }

        public bool InRun = false;

        private IList<Geopoint> _Waypoints;

        protected ManualResetEvent _DataUpdateSyncEvent;

        internal event EventHandler<MapRoute> OnRouteUpdate;
        public event Action<string> OnVoiceMsgUpdate;
        private ManualResetEvent speechEvent;
        MediaElement mediaPlayer = new MediaElement();
        private bool changed;

        public RunManager()
        {
            _DataUpdateSyncEvent = new ManualResetEvent(true);

            RunData = new RunItem();
            speechEvent = new ManualResetEvent(true);

        }

        public Timer aTimer;
        public Timer bTimer;
        public bool solorun = true;


        internal virtual void Start()
        {
            InRun = true;
            RunData.Date = DateTime.Now;
            bTimer = new Timer(CallbackB, null, 0, 30000);
            //  bTimer = new Timer(Callback, null, 0, 300000); TODO CHANGE TO THIS IN PROD
            aTimer = new Timer(Callback, null, 0, 1);


            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            changed = false;
            _Waypoints = new List<Geopoint>();

            LocationService.Instance.Start();
            if (solorun)
            {
                VoiceMsg = "activity started";
            }
        }

        public void Callback(object state)
        {
            RunData.Time = DateTime.Now - RunData.Date;
        }

        public virtual void CallbackB(object state)
        {
            if (InRun)
            {

                VoiceMsg = "Time: " + RunData.Time.Minutes + "minutes" + RunData.Time.Seconds + "seconds . Distance: " +
                           RunData.Distance
                           + " kilometer. Speed: " + RunData.Speed
                           + "kilometer per hour";
            }
        }

        internal virtual void Stop()
        {
            RunData.Speed = RunData.Time.Seconds != 0 ? RunData.Distance / (RunData.Time.Seconds / 60 / 60) : 0;
            if (double.IsNaN(RunData.Speed)) RunData.Speed = 0;
            VoiceMsg = "activity stopped. Run summery. Time: " + RunData.Time.Minutes + "minutes" + RunData.Time.Seconds + "seconds . Distance: " + RunData.Distance
            + " kilometer. Average speed: " + RunData.Speed
            + "kilometer per hour";
            InRun = false;
            RunData.Time = TimeSpan.Zero;

            aTimer.Dispose();
            bTimer.Dispose();
            LocationService.Instance.Stop();
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
            MobileService.Instance.SaveRunData(RunData);
            RunData = new RunItem();
        }

        protected virtual async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            await UpdateRunStats(obj);

            _DataUpdateSyncEvent.Set();
        }

        protected async Task UpdateRunStats(Geoposition obj)
        {
            _Waypoints.Add(obj.ToGeoPoint());

            var route = await MapServiceWrapper.Instance.GetRoute(_Waypoints);

            if (route != null)
            {
                OnRouteUpdate?.Invoke(this, route);
                double distanceDiff = route.LengthInMeters - RunData.Distance;
                RunData.Distance = route.LengthInMeters;
                RunData.Speed = (distanceDiff / 1000) / (_lastLocationTimeSeconds / 60.0 / 60.0);
                _lastLocationTimeSeconds = RunData.Time.Seconds;

            }

            UpdateVoiceMsg();
        }


        public void UpdateVoiceMsg()
        {
            if (BandService.Instance.HeartRate < BandService.Instance.MinTargetZoneHeartRate && !changed)
            {
                changed = true;
                VoiceMsg = "You are not in your target Heart rate. The minimum target Heart rate is " +
                           BandService.Instance.MinTargetZoneHeartRate.ToString() + " beats per seconds. speed up!";
            }
            else if (BandService.Instance.HeartRate > BandService.Instance.MinTargetZoneHeartRate && changed)
            {
                changed = false;
                VoiceMsg = "Good job " + MobileService.Instance.UserData.PrivateName +
                            "! you are in your target Heart Rate!  .the minimum target Heart rate is " + BandService.Instance.MinTargetZoneHeartRate.ToString() + " beats per seconds and up";
            }


        }



        public async Task ReadText(string text)
        {
            speechEvent.WaitOne();
            speechEvent.Reset();
            mediaPlayer.Stop();

            using (var speach = new SpeechSynthesizer())
            {
                speach.Voice = SpeechSynthesizer.AllVoices.First(i => (i.Gender == VoiceGender.Female));
                SpeechSynthesisStream stream = await speach.SynthesizeTextToStreamAsync(text);
                mediaPlayer.SetSource(stream, stream.ContentType);
                mediaPlayer.Play();
            }

            speechEvent.Set();
        }
    }
}
