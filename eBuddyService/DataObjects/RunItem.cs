using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace eBuddyService.DataObjects
{
    public class RunItem : EntityData
    {        
        public String FacebookId { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public TimeSpan Time { get; set; }
        public double Speed { get; set; }
    }
}