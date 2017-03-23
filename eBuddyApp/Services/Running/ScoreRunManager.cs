﻿using System;
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
    internal class ScoreRunManager
    {

        private int _lastLocationTimeSeconds = 1;
        private const int CHILL_TIME = 20;
        private const int PRE_RUN_TIME = 10;
        private const int WARM_UP_DISTANCE = 1600;
        private const int INTENSE_DISTANCE = 1200;      

        private TimeSpan _TimeBeforeIntense;
        private TimeSpan _TimeBeforePreRun;
        private double _DistanceBeforeWarmUp;
        private int _MinHeartrate = int.MaxValue;
        private int _MaxHeartrate = 0;
        private double _finalScore = 0;
        public double finalScore
        {
            get { return _finalScore; }
            private set { _finalScore = value; }
        }

        private double _score_by_mas = 0;
        public double score_by_mas
        {
            get { return _score_by_mas; }
            private set { _score_by_mas = value;}
        }
        private double _score_by_vo2max = 0;
        public double score_by_vo2max
        {
            get { return _score_by_vo2max; }
            private set { _score_by_vo2max = value; }
        }
        //private double _DistanceBeforeIntense;
        private List<int> _HeartratePreRunList;
        private  static ScoreRunManager _scoreRunInstance;

        public static ScoreRunManager scoreRunInstance
        {
            get
            {
                if (_scoreRunInstance == null)
                {
                    _scoreRunInstance = new ScoreRunManager();
                }

                return _scoreRunInstance;
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
        public  ScoreRunManager() 
        {
            
            _DataUpdateSyncEvent = new ManualResetEvent(true);

            RunData = new RunItem();
            RunData.FacebookId = MobileService.Instance.UserData.FacebookId;
            
            _HeartratePreRunList = new List<int>();
        }

        public Timer aTimer;
        public enum ERunPhase
        {
            NotStarted,
            Chill,
            WarmUp,
            PreRun,
            Intense,
            Finished
        }

        public event EventHandler<ERunPhase> OnRunPhaseChange;
        private ERunPhase _RunPhase;
        public ERunPhase RunPhase
        {
            get { return _RunPhase; }
            protected set
            {
                _RunPhase = value;

                OnRunPhaseChange?.Invoke(this, value);
            }
        }

        internal  void Start()
        { 
            RunPhase = ERunPhase.Chill;

            InRun = true;
            aTimer = new Timer(Callback, null, 0, 1);
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;

            RunData.Date = DateTime.Now;
            _Waypoints = new List<Geopoint>();

            LocationService.Instance.Start();

            
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
        }
        private void Callback(object state)
        {
            RunData.Time = DateTime.Now - RunData.Date;

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

        internal async Task Stop()
        {
            InRun = false;
            RunData.Time = TimeSpan.Zero;
            aTimer.Dispose();
            LocationService.Instance.Stop();
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
            
        
            if (RunPhase == ERunPhase.Finished)
            {
                RunData.Speed = RunData.Time.Seconds != 0 ? RunData.Distance / (RunData.Time.Seconds / 60.0 / 60) : 0;
                if (double.IsNaN(RunData.Speed)) RunData.Speed = 0;
                MobileService.Instance.SaveRunData(RunData);
             await MobileService.Instance.SaveUserScore(finalScore);
                RunData = new RunItem();

            }
            RunPhase = ERunPhase.NotStarted;
        }

        protected  async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            await UpdateRunStats(obj);

            UpdateTestPhase();

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
        }

        private async Task UpdateTestPhase()
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
                        if (RunData.Time.TotalSeconds >= 50)
                        {
                            RunData.Distance = 1610;
                        }


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
                        if (RunData.Time.TotalSeconds >= 80)
                        {
                            RunData.Distance = 1610;
                        }
                        if (RunData.Distance >= INTENSE_DISTANCE)
                        {

                            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;

                            RunPhase++;

                            LocationService.Instance.Stop();
                            finalScore = CalculateScore();

                           await Stop();
                            

                        }
                        
                        break;
                    }
                case ERunPhase.Finished:
                    {
                       
                        
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

            score_by_mas = (mas_avg / 6.22) * 100;
            score_by_vo2max = (vo2max_avg / 70.0) * 100;

            
            return (score_by_mas + score_by_vo2max) / 2;
        }

 
    }
}
