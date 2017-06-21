using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    internal sealed class BIDDING: BID
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

    }
}
