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

        [JsonProperty(PropertyName = "privatename")]
        public String PrivateName { get; set; }

        [JsonProperty(PropertyName = "surname")]
        public String SurName { get; set; }

        [JsonProperty(PropertyName = "age")]
        public Double Age { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public Double Weight { get; set; }

        [JsonProperty(PropertyName = "height")]
        public Double Height { get; set; }

        [JsonProperty(PropertyName = "mas")]
        public Double MAS { get; set; }
    }
}
