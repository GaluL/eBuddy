using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eBuddy.DataModel
{
    class UserItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "privateName")]
        public String PrivateName { get; set; }

        [JsonProperty(PropertyName = "surName")]
        public String SurName { get; set; }

        [JsonProperty(PropertyName = "age")]
        public Double Age { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public Double Weight { get; set; }

        [JsonProperty(PropertyName = "height")]
        public Double Height { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public Boolean Gender { get; set; }

        [JsonProperty(PropertyName = "email")]
        public String Email { get; set; }

        [JsonProperty(PropertyName = "FacebookId")]
        public String FacebookId { get; set; }
    }
}
