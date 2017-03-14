using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Template10.Utils;

namespace eBuddy
{
    internal class ScoreRunManager : RunManager
    {
        public enum ERunPhase
        {
            NotStarted,
            Chill,
            WarmUp,
            PreRun,
            Intense,
            Finished
        }

        private const int CHILL_TIME = 20;
        private const int PRE_RUN_TIME = 10;
        private const int WARM_UP_DISTANCE = 1600;
        private const int INTENSE_DISTANCE = 1200;      

        private TimeSpan _TimeBeforeIntense;
        private TimeSpan _TimeBeforePreRun;
        private double _DistanceBeforeWarmUp;
        private int _MinHeartrate = int.MaxValue;
        private int _MaxHeartrate = 0;
        //private double _DistanceBeforeIntense;
        private List<int> _HeartratePreRunList;

        public event EventHandler<ERunPhase> OnRunPhaseChange; 
        private ERunPhase _RunPhase;
        public ERunPhase RunPhase
        {
            get { return _RunPhase; }
            private set
            {
                _RunPhase = value;

                OnRunPhaseChange?.Invoke(this, value);
            }
        }

        private ScoreRunManager() : base()
        {
            _HeartratePreRunList = new List<int>();
        }

        internal override void Start()
        { 
            RunPhase = ERunPhase.Chill;

            base.Start();

            LocationService.Instance.OnLocationChange += this.Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
        }

        private void Instance_OnHeartRateChange(object sender, int e)
        {
            if (RunPhase == ERunPhase.Chill && e < _MinHeartrate)
            {
                _MinHeartrate = e;
            }
            else if (RunPhase == ERunPhase.Intense && e > _MaxHeartrate)
            {
                _MaxHeartrate = e;
            }
            else if (RunPhase == ERunPhase.PreRun)
            {
                _HeartratePreRunList.Add(e);
            }
        }

        internal override void Stop()
        {
            base.Stop();

            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
            
            RunPhase = ERunPhase.NotStarted;
        }

        protected override async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            await UpdateRunStats(obj);

            UpdateTestPhase();

            _DataUpdateSyncEvent.Set();
        }

        private void UpdateTestPhase()
        {
            switch (RunPhase)
            {
                case ERunPhase.Chill:
                {
                    if (RunData.Time.TotalSeconds >= CHILL_TIME)
                    {
                        _DistanceBeforeWarmUp = RunData.Distance;
                        RunPhase++;
                    }

                    break;
                }
                case ERunPhase.WarmUp:
                    {
                        if (RunData.Distance >= _DistanceBeforeWarmUp + WARM_UP_DISTANCE)
                        {
                            _TimeBeforePreRun = RunData.Time;
                            RunPhase++;
                        }

                        break;
                    }
                case ERunPhase.PreRun:
                    {
                        if (RunData.Time.TotalSeconds >= _TimeBeforePreRun.TotalSeconds + PRE_RUN_TIME)
                        {
                            RunPhase++;
                            _TimeBeforeIntense = RunData.Time;
                            //_DistanceBeforeIntense = RunData.Distance;
                            RunData.Distance = 0;
                            RunData.Time = TimeSpan.Zero;
                        }

                        break;
                    }
                case ERunPhase.Intense:
                    {
                        if (RunData.Distance >= INTENSE_DISTANCE)
                        {

                            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                            RunPhase++;

                            LocationService.Instance.Stop();

                            CalculateScore();
                        }

                        break;
                    }
            }
        }

        private double CalculateScore()
        {
            double vo2max_hr_based = 15.3 * (_MaxHeartrate / (double)_MinHeartrate);
            double avg_hr_prerun = _HeartratePreRunList.Average();
            double vo2max_measures_based = 132.853 - (0.0769 * 2.204 * MobileService.Instance.UserData.Weight) -
                                           (0.387 * MobileService.Instance.UserData.Weight) +
                                           (6.315 * (MobileService.Instance.UserData.Gender.Value ? 1 : 0) -
                                           (3.2649 * (_TimeBeforeIntense.Seconds - _TimeBeforePreRun.Seconds) / 60.0) -
                                           0.1565 * avg_hr_prerun);

            double vo2max_avg = (vo2max_measures_based + vo2max_hr_based) / 2;
            double mas_vo2max_based = vo2max_avg / 3.5;
            double mas_run_based;

            if (MobileService.Instance.UserData.Weight > 100)
            {
                mas_run_based = INTENSE_DISTANCE / (double)(RunData.Time.Seconds - 29);
            }
            else
            {
                mas_run_based = INTENSE_DISTANCE / (RunData.Time.Seconds - 20.3);
            }

            double mas_avg = (mas_run_based + mas_vo2max_based) / 2;

            double score_by_mas = mas_avg / 6.22 * 100;
            double score_by_vo2max = vo2max_avg / 70.0 * 100;

            return (score_by_mas + score_by_vo2max) / 2;
        }
    }
}
