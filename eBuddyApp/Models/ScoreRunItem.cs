using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eBuddyApp.Models
{
    class ScoreRunItem : RunItem
    {
        ERunPhase _RunPhase = ERunPhase.NotStarted;
        [JsonProperty(PropertyName = "runPhase")]
        public ERunPhase RunPhase { get { return _RunPhase; } set { Set(ref _RunPhase, value); } }

        double _Score = default(double);
        [JsonProperty(PropertyName = "score")]
        public double Score { get { return _Score; } set { Set(ref _Score, value); } }
    }
}
