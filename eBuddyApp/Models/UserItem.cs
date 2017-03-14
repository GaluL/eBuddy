using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Template10.Mvvm;

namespace eBuddyApp.Models
{
    class UserItem : BindableBase
    {
        public string Id { get; set; }

        private String _PrivateName = default(string);
        [JsonProperty(PropertyName = "privatename")]
        public String PrivateName { get { return _PrivateName; } set { Set(ref _PrivateName, value); } }

        private String _SurName = default(string);
        [JsonProperty(PropertyName = "surname")]
        public String SurName { get { return _SurName; } set { Set(ref _SurName, value); } }

        private Double _Age = default(Double);
        [JsonProperty(PropertyName = "age")]
        public Double Age { get { return _Age; } set { Set(ref _Age, value); } }

        private Double _Weight = default(Double);
        [JsonProperty(PropertyName = "weight")]
        public Double Weight { get { return _Weight; } set { Set(ref _Weight, value); } }

        private Double _Height = default(Double);
        [JsonProperty(PropertyName = "height")]
        public Double Height { get { return _Height; } set { Set(ref _Height, value); } }

        private Boolean? _Gender = default(bool);
        [JsonProperty(PropertyName = "gender")]
        public Boolean? Gender { get { return _Gender; } set { Set(ref _Gender, value); } }

        private String _Email = default(string);
        [JsonProperty(PropertyName = "email")]
        public string Email { get { return _Email; } set { Set(ref _Email, value); } }

        private String _FacebookId = default(string);
        [JsonProperty(PropertyName = "FacebookId")]
        public string FacebookId { get { return _FacebookId; } set { Set(ref _FacebookId, value); } }
    }
}
