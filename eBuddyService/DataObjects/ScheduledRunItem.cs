using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace eBuddyService.DataObjects
{
    public class ScheduledRunItem : EntityData
    {
        public String InitializerFacebookId { get; set; }
        public String BuddyFacebookId { get; set; }
        public bool BuddyApproval { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public bool Finished { get; set; }
        public string Winner { get; set; }
    }
}