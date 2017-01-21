﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBuddy
{
    class TestRunManager : RunManager
    {
        // The mode
        private const int ChillMode = 1;
        private const int WarmUpMode = 2;
        private const int InRunMode = 3;
        private const int FinishedMode = 4;

        private const int restHeartBeatTime = 20;
        private const int warmUpTime = 20;
        private const double v02maxMultiplayer = 15.3;
        private const double testDistance = 1200;



        private double _maxHeartrate = 0;
        private double _restHeartrate = 0;
        private TimeSpan modeTime;
        private DateTime _testStartTime;
        private double _totalDistance;
        private double _maxSpeed = 0;
        private double v02Max = 0;
        private double MAS = 0;
        /*
         * Indicate and the TestRun status - 
         *                                   0 - Before started
         *                                   1 - Chill - 20 second chillout time in this
         *                                       time the will sampe the rest heart beat.
         *                                   2 - Warm up 20 minutes of warm up before the run.
         *                                   3 - In run - 1200 meters of high pace running
         *                                   4 - Finished - calculate and present score.   
         */
        private int _Status = 0;

        // Constructor
        public TestRunManager()
        {


            base.OnHeartRateUpdate += TestRun_OnHeartRateUpdate;
            base.OnDistanceUpdate += TestRun_OnDistanceUpdate;
            base.OnTimeUpdate += TestRun_OnTimeUpdate;
            base.OnSpeedUpdate += TestRun_OnSpeedUpdate;


        }

        private void TestRun_OnHeartRateUpdate(double obj)
        {
            if (Heartrate > MaxHeartrate)
            {
                MaxHeartrate = Heartrate;
            }
            if (Status == ChillMode)
            {
                RestHeartrate = Heartrate;
            }

        }

        private void TestRun_OnSpeedUpdate(double obj)
        {
            switch (Status)
            {
                case InRunMode:
                    if (MaxSpeed < Speed)
                    {
                        MaxSpeed = Speed;

                    }
                    break;
            }
        }

        private void TestRun_OnTimeUpdate(TimeSpan obj)
        {
            switch (Status)
            {
                case WarmUpMode:
                    if (obj.Minutes == warmUpTime)
                    {
                        base.Stop();
                        Status = InRunMode;
                        // starting the intense run
                        base.Start();
                    }
                    break;
            }
        }

        private void TestRun_OnDistanceUpdate(double obj)
        {
            switch (Status)
            {
                case InRunMode:
                    if (obj == testDistance)
                    {
                        Status = FinishedMode;
                        ModeTime = Time;
                        Stop();

                    }
                    break;
            }
        }





        public double MaxHeartrate
        {
            get
            {
                return _maxHeartrate;
            }

            private set
            {
                _maxHeartrate = value;
            }
        }

        public double RestHeartrate
        {
            get
            {
                return _restHeartrate;
            }

            private set
            {
                _restHeartrate = value;
                OnHeartRestRateUpdate?.Invoke(value);
            }
        }

        protected TimeSpan ModeTime
        {
            get
            {
                return modeTime;
            }

            private set
            {
                modeTime = value;
                OnModeTimeUpdate?.Invoke(value);
            }
        }

        public int Status
        {
            get
            {
                return _Status;
            }

            private set
            {
                _Status = value;
                OnStatusChanged?.Invoke(value);
            }
        }

        public DateTime TestStartTime
        {
            get
            {
                return _testStartTime;
            }

            private set
            {
                _testStartTime = value;
            }
        }

        public double TotalDistance
        {
            get
            {
                return _totalDistance;
            }

            private set
            {
                _totalDistance = value;
                OnTotalDistanceChanged?.Invoke(value);
            }
        }

        public double MaxSpeed
        {
            get
            {
                return _maxSpeed;
            }

            private set
            {
                _maxSpeed = value;
            }
        }

        public double V02Max
        {
            get
            {
                return v02Max;
            }

            set
            {
                v02Max = value;
            }
        }

        public double MASscore
        {
            get
            {
                return MAS;
            }

            set
            {
                MAS = value;
            }
        }

        public event Action<int> OnStatusChanged;
        public event Action<TimeSpan> OnModeTimeUpdate;
        public event Action<double> OnTotalDistanceChanged;
        public event Action<double> OnHeartRestRateUpdate;

        // Override the start method
        internal override void Start()
        {
            Status = ChillMode;
            Waypoints.Clear();
            TestStartTime = DateTime.Now;
            ModeTime = DateTime.Now - TestStartTime;

            while (ModeTime.Seconds <= restHeartBeatTime)
            {
                ModeTime = DateTime.Now - TestStartTime;
                // TODO: update the user countdown
            }
            Status = WarmUpMode;

            // starting the warm up -> UI need to tell the user to start
            // warm up for the next 20 minutes it could be light run or straches.
            base.Start();
        }

        internal override void Stop()
        {
            RestHeartrate = RestHeartrate * 3;

            V02Max = (MaxHeartrate / RestHeartrate) * v02maxMultiplayer;
            MASscore = (testDistance / modeTime.Seconds);

            LocationTracker.Instance.Stop();

        }
    }
}
