using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Template10.Mvvm;

namespace eBuddyApp.Models
{
    class LiveRunStats : BindableBase
    {
        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        double _Speed = default(double);
        public double Speed { get { return _Speed; } set { Set(ref _Speed, value); } }

        private MapRoute _Route = default(MapRoute);
        public MapRoute Route { get { return _Route; } set { Set(ref _Route, value); } }

        int _Heartrate = default(int);
        public int Heartrate { get { return _Heartrate; } set { Set(ref _Heartrate, value); } }

        TimeSpan _Time = default(TimeSpan);
        public TimeSpan Time { get { return _Time; } set { Set(ref _Time, value); } }

        bool _InProgress = false;
        public bool InProgress { get { return _InProgress; } set { Set(ref _InProgress, value); } }
    }
}
