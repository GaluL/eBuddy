using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBuddyService.DataObjects
{
    public class eBuddyUser1 : EntityData
    {
        public string Id { get; set; }

        public string name { get; set; }

        public int age { get; set; }

        public int height { get; set; }

        public int weight { get; set; }
    }
}