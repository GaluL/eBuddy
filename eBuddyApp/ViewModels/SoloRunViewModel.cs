using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Media.SpeechSynthesis;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace eBuddyApp.ViewModels
{
    class SoloRunViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;

        #endregion

        #region Properties

        public Page page;
        private ManualResetEvent speechEvent;
        MediaElement mediaPlayer = new MediaElement();

        public bool IsScoreRun { get; set; }

        public virtual RunItem RunData
        {
            get { return RunManager.Instance.RunData; }
        }

        public int _Heartrate;
        public int Heartrate { get { return _Heartrate; } private set { Set(ref _Heartrate, value); } }

        public MapRoute _MyRoute;
        public MapRoute MyRoute { get { return _MyRoute; } private set { Set(ref _MyRoute, value); } }

        public Geopoint _CurrentLocation;
        public Geopoint CurrentLocation { get { return _CurrentLocation; }
            set { Set(ref _CurrentLocation, value); } }

        #endregion

        public SoloRunViewModel() : base()
        {
            StartRun = new RelayCommand(() => {
                    RunManager.Instance.Start();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    RunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
                    RunManager.Instance.OnVoiceMsgUpdate += Instance_OnMsgUpdate;
            }, 
                () => { return (!RunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
                {
                    RunManager.Instance.ReadText("Stoped");
                    RunManager.Instance.Stop();  
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                    RunManager.Instance.OnRouteUpdate -= Instance_OnRouteUpdate;
                    LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
                    BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                    RunManager.Instance.OnVoiceMsgUpdate -= Instance_OnMsgUpdate;
                },
                () => { return RunManager.Instance.InRun; });

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
            speechEvent = new ManualResetEvent(true);

        }

        public void Instance_OnHeartRateChange(object sender, int e)
        {
            Heartrate = e;
        }

        public void Instance_OnLocationChange(Geoposition obj)
        {
            CurrentLocation = obj.ToGeoPoint();
        }

        public void Instance_OnRouteUpdate(object sender, MapRoute e)
        {
            MyRoute = e;
        }

        private async void Instance_OnMsgUpdate(string obj)
        {
            await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await RunManager.Instance.ReadText(obj);
            });

        }

        public async Task ReadText(string text)
        {
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

