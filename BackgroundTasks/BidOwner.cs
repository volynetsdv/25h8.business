using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class BidOwner
    {
        [JsonProperty(PropertyName = "owner")]
        public object Owner { get; set; }

        public BidOwner() { }

        public BidOwner(object owner)
        {
            Owner = owner;

        }
    }
}
