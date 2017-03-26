using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using WeatherNet.Model;

namespace eBuddyApp.ViewModels
{
    class ScoreRunViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;
        

        #endregion

        #region Properties

        public Page page;
        public bool IsScoreRun { get; set; }
        public RunItem RunData { get { return ScoreRunManager.scoreRunInstance.RunData; } }


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

        private CurrentWeatherResult _CurrentWeather;
        public CurrentWeatherResult CurrentWeather
        {
            get { return _CurrentWeather; }
            set
            {
                _CurrentWeather = value;
                RaisePropertyChanged("CurrentWeather");
            }
        }
        #endregion
       
        private bool inTalk = false;
        private string VoiceMsg;
        private ManualResetEvent speechEvent;
        MediaElement mediaPlayer = new MediaElement();

        public ScoreRunViewModel() : base()
        {
            StartRun = new RelayCommand(() =>
                {
                
                BandService.Instance.OnConnectionStatusChange += Instance_OnConnectionStatusChange;
                ScoreRunManager.scoreRunInstance.OnRunPhaseChange += Instance_OnRunPhaseChange;
                ScoreRunManager.scoreRunInstance.OnRouteUpdate += Instance_OnRouteUpdate;
                LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
                BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
                

                ScoreRunManager.scoreRunInstance.Start();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();

            },
                () =>
                {
                    return (!ScoreRunManager.scoreRunInstance.InRun); /*&& BandService.Instance.IsConnected*/

                });

            StopRun = new RelayCommand(() =>
                {

                
                ScoreRunManager.scoreRunInstance.Stop();
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
                stopPressed();
                },
                () =>
                {
                    return (ScoreRunManager.scoreRunInstance.InRun);

                });

            CurrentLocation = ExtentionMethods.GetDefaultPoint();

            speechEvent = new ManualResetEvent(true);
            ScoreRunManager.scoreRunInstance.OnInRunChange += ScoreRunInstance_OnInRunChange;
            ScoreRunManager.scoreRunInstance.OnIsFinishedChange += ScoreRunInstance_OnIsFinishedChange;
        }

        private void ScoreRunInstance_OnInRunChange(object sender, bool e)
        {
            if (e)
            {
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
            }
            else
            {
                ScoreRunManager.scoreRunInstance.OnRunPhaseChange -= Instance_OnRunPhaseChange;
                BandService.Instance.OnConnectionStatusChange -= Instance_OnConnectionStatusChange;
                ScoreRunManager.scoreRunInstance.OnRouteUpdate -= Instance_OnRouteUpdate;
                LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
                BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                StopRun.RaiseCanExecuteChanged();
                StartRun.RaiseCanExecuteChanged();
                
            }
            
            
        }

        private async void ScoreRunInstance_OnIsFinishedChange(object sender, bool e)
        {
            if (e)
            {
                VoiceMsg = "scoreRun completed. Run summery. your score is: " +
                           ScoreRunManager.scoreRunInstance.finalScore;
                await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await ReadText(VoiceMsg);
                });
            }          
        }


        private async void stopPressed()
        {
                VoiceMsg = "you did not finished the scoreRun. the score was not calculated";
                await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                 {
                await ReadText(VoiceMsg);
                 });
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


        public async void Instance_OnRunPhaseChange(object sender, ScoreRunManager.ERunPhase value)
        {
            await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                switch (value)
                {
                    case ScoreRunManager.ERunPhase.NotStarted:
                        {
                            //  await ReadText("Score mode");
                            break;
                        }
                    case ScoreRunManager.ERunPhase.Chill:
                        {

                            await ReadText(
                                "You are now in chill mode, please chill for twenty second while we take some measurement");
                            break;
                        }
                    case ScoreRunManager.ERunPhase.WarmUp:
                        {
                            await ReadText("You are now in warmup mode, go for 1.5 kilometers walk as fast as you can");
                            break;
                        }
                    case ScoreRunManager.ERunPhase.PreRun:
                        {
                            await ReadText(
                                "prepare yourself, in ten seconds your about to go for inetnse run for 1.2 kilometers");
                            break;
                        }
                    case ScoreRunManager.ERunPhase.Intense:
                        {
                            await ReadText("Go go go! Run as fast as you can for 1.2 kilometers");
                            break;
                        }
                    case ScoreRunManager.ERunPhase.Finished:
                        {
                            await ReadText("good job!..... we are calculating your score");
                            break;
                        }
                }

            });
        }

    }
}

