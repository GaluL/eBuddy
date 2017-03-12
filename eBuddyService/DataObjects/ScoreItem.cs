using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace eBuddyService.DataObjects
{
    public class ScoreItem : EntityData
    {
        public Double MAS { get; set; }
        public Double VO2MAX { get; set; }
        public String FacebookId { get; set; }
    }
}