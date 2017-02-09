using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace eBuddyService.DataObjects
{
    public class UserItem : EntityData
    {
        public String PrivateName { get; set; }
        public String SurName { get; set; }
        public Double Age { get; set; }
        public Double Weight { get; set; }
        public Double Height { get; set; }
        public Boolean Gender { get; set; }
        public String Email { get; set; }
        public String FacebookId { get; set; }
    }
}