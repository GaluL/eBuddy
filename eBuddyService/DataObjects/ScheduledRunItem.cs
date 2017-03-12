using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace eBuddyService.DataObjects
{
    public class ScheduledRunItem : EntityData
    {        
        public String User1FacebookId { get; set; }
        public String User2FacebookId { get; set; }
        public bool User1Approved { get; set; }
        public bool User2Approved { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public bool Finished { get; set; }
        public string Winner { get; set; }
    }
}