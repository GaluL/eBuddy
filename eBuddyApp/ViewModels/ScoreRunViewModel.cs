using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eBuddyApp.ViewModels
{
    class ScoreRunViewModel: ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;

        #endregion

        #region Properties

        public Page page;
        public bool IsScoreRun { get; set; }
        public  RunItem RunData { get { return ScoreRunManager.scoreRunInstance.RunData; } }


        public int _Heartrate;
        public int Heartrate { get { return _Heartrate; } private set { Set(ref _Heartrate, value); } }

        public MapRoute _MyRoute;
        public MapRoute MyRoute { get { return _MyRoute; } private set { Set(ref _MyRoute, value); } }

        public Geopoint _CurrentLocation;
        public Geopoint CurrentLocation
        {
            get { return _CurrentLocation; }
            set { Set(ref _CurrentLocation, value); }
        }
        #endregion

        public ScoreRunViewModel(): base()
        {
            StartRun = new RelayCommand(() => {
                
                ScoreRunManager.scoreRunInstance.Start();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
                
            },
            () => { return (!ScoreRunManager.scoreRunInstance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
            {
                ScoreRunManager.scoreRunInstance.Stop();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
                ScoreRunManager.scoreRunInstance.OnRunPhaseChange -= Instance_OnRunPhaseChange;
            },
            () => { return ScoreRunManager.scoreRunInstance.InRun; });

            BandService.Instance.OnConnectionStatusChange += Instance_OnConnectionStatusChange;
            ScoreRunManager.scoreRunInstance.OnRunPhaseChange += Instance_OnRunPhaseChange;
            ScoreRunManager.scoreRunInstance.OnRouteUpdate += Instance_OnRouteUpdate;
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
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
        private void Instance_OnConnectionStatusChange(bool obj)
        {
            StartRun.RaiseCanExecuteChanged();
        }
        public async void ReadText(string text)
        {
             
             MediaElement mediaPlayer = new MediaElement();
            using (var speach = new SpeechSynthesizer())
            {
                speach.Voice = SpeechSynthesizer.AllVoices.First(i => (i.Gender == VoiceGender.Female));
                SpeechSynthesisStream stream = await speach.SynthesizeTextToStreamAsync(text);
                mediaPlayer.SetSource(stream, stream.ContentType);
                mediaPlayer.Play();
            }
        }


        public async void Instance_OnRunPhaseChange(object sender, ScoreRunManager.ERunPhase value)
        {
            await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {


                 switch (value)
                 {
                     case ScoreRunManager.ERunPhase.NotStarted:
                         {
                            // ReadText("Score mode");
                             break;
                         }
                     case ScoreRunManager.ERunPhase.Chill:
                         {
                             ReadText(
                                "You are now in chill mode, please chill for twenty second while we take some measurement");
                             break;
                         }
                     case ScoreRunManager.ERunPhase.WarmUp:
                         {
                             ReadText("You are now in warmup mode, go for 1.5 kilometers walk as fast as you can");
                             break;
                         }
                     case ScoreRunManager.ERunPhase.PreRun:
                         {
                             ReadText("prepare yourself, in ten seconds your about to go for inetnse run for 1.2 kilometers");
                             break;
                         }
                     case ScoreRunManager.ERunPhase.Intense:
                         {
                             ReadText("Gooooo! Run as fast as you can for 1.2 kilometers");
                             break;
                         }
                     case ScoreRunManager.ERunPhase.Finished:
                         {
                             ReadText("good job!..... we are calculating your score");
                             break;
                         }
                 }
             });
        }

    }
  }

