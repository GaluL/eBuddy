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
using Windows.UI.Xaml.Media;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Microsoft.WindowsAzure.MobileServices;
using Template10.Common;
using Template10.Samples.SearchSample.Controls;

namespace eBuddy
{
    internal class RunManager
    {
        private static RunManager _Instance;
        private int _cnt = 0;

        protected ManualResetEvent _DataUpdateSyncEvent;
        private double _lastLocationTimeSeconds = 1;

        private RunItem _RunData;
        private string _voiceMsg;

        private IList<Geopoint> _Waypoints;

        public Timer aTimer;

        public Timer bTimer;
        private bool changed;

        public bool InRun = false;
        private MediaElement mediaPlayer = new MediaElement();
        public bool solorun = true;
        private ManualResetEvent speechEvent;

        public RunManager()
        {
            _DataUpdateSyncEvent = new ManualResetEvent(true);
            _Waypoints = new List<Geopoint>();
            RunData = new RunItem();
            speechEvent = new ManualResetEvent(true);
        }

        public static RunManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new RunManager();

                return _Instance;
            }
        }

        public string VoiceMsg
        {
            get { return _voiceMsg; }
            set
            {
                _voiceMsg = value;
                OnVoiceMsgUpdate?.Invoke(value);
            }
        }

        public RunItem RunData
        {
            get { return _RunData; }
            set { _RunData = value; }
        }

        public int MaxHeartRate { get; internal set; }

        internal event EventHandler<MapRoute> OnRouteUpdate;
        public event Action<string> OnVoiceMsgUpdate;
        public event Action<object> OnPopped;

        internal virtual async void Start()
        {
            RunData.Distance = 0;
            RunData.Speed = 0;
            RunData.Time = TimeSpan.Zero;
            InRun = true;
            RunData.Date = DateTime.Now;
            bTimer = new Timer(CallbackB, null, 0, 60000);
            aTimer = new Timer(Callback, null, 0, 1);


            _Waypoints.Clear();
            MaxHeartRate = 0;
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            changed = false;

            LocationService.Instance.Start();
            if (solorun)
                await ReadText("activity started");
        }

        public void Callback(object state)
        {
            RunData.Time = DateTime.Now - RunData.Date;
            if (MaxHeartRate < BandService.Instance.HeartRate)
                MaxHeartRate = BandService.Instance.HeartRate;
        }


        public virtual void CallbackB(object state)
        {
            if (InRun)
                VoiceMsg = "Time: " + RunData.Time.Minutes + "minutes" + RunData.Time.Seconds + "seconds . Distance: " +
                           RunData.Distance
                           + " meters. Speed: " + RunData.Speed
                           + " kilometer per hour";
        }

        internal virtual void Stop()
        {
            RunData.Speed = RunData.Time.Seconds != 0
                ? (RunData.Distance / 1000) / (RunData.Time.Seconds / 60.0 / 60)
                : 0;
            if (double.IsNaN(RunData.Speed)) RunData.Speed = 0;
            VoiceMsg = "activity completed. Run summery. Time: " + RunData.Time.Minutes + "minutes" +
                       RunData.Time.Seconds +
                       "seconds . Distance: " + RunData.Distance
                       + " meters. Average speed: " + RunData.Speed
                       + " kilometer per hour";
            InRun = false;
            aTimer.Dispose();
            bTimer.Dispose();
            LocationService.Instance.Stop();
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
            MobileService.Instance.SaveRunData(RunData);
            _lastLocationTimeSeconds = 1;
            OnPopped?.Invoke(true);
        }

        protected virtual async void Instance_OnLocationChange(Geoposition obj)
        {
            if (InRun)
                await UpdateRunStats(obj);
        }

        protected async Task UpdateRunStats(Geoposition obj)
        {
            if (InRun)
            {
                _Waypoints.Add(obj.ToGeoPoint());

                var route = await MapServiceWrapper.Instance.GetRoute(_Waypoints);

                if (route != null)
                {
                    OnRouteUpdate?.Invoke(this, route);

                    double distanceDiff = route.LengthInMeters - RunData.Distance;
                    RunData.Distance = route.LengthInMeters;
                    if (distanceDiff > 0 || _cnt > 9)
                        RunData.Speed = (distanceDiff / 1000) /
                                        ((RunData.Time.TotalSeconds - _lastLocationTimeSeconds) / 60.0 / 60.0);
                    if (distanceDiff > 0)
                        _cnt = 0;
                    else
                        _cnt++;

                    _lastLocationTimeSeconds = RunData.Time.TotalSeconds;
                }
                else
                {
                    _Waypoints.RemoveAt(_Waypoints.Count - 1);
                }

                UpdateVoiceMsg();
            }
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
                           "! you are in your target Heart Rate! the minimum target Heart rate is " +
                           BandService.Instance.MinTargetZoneHeartRate.ToString() + " beats per seconds and up";
            }
        }


        public async Task ReadText(string text)
        {
            speechEvent.WaitOne(2);
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