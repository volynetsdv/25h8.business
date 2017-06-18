using Newtonsoft.Json;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTasks
{
    class Deserialize
    {
        public class BID
        {
            [JsonProperty(PropertyName = "entityType")]
            public string entityType { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string title { get; set; }
            [JsonProperty(PropertyName = "proc")]
            public string proc { get; set; }
            [JsonProperty(PropertyName = "contractorName")]
            public string contractorName { get; set; }
            [JsonProperty(PropertyName = "LogogURL")]
            public string LogogURL { get; set; }
            [JsonProperty(PropertyName = "Id")]
            public int Id { get; set; }
        }
        public sealed class BIDDING
        {
            [JsonProperty(PropertyName = "entityType")]
            public string entityType { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string title { get; set; }
            [JsonProperty(PropertyName = "proc")]
            public string proc { get; set; }
            [JsonProperty(PropertyName = "contractorName")]
            public string contractorName { get; set; }
            [JsonProperty(PropertyName = "LogogURL")]
            public string LogogURL { get; set; }
            [JsonProperty(PropertyName = "Id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "state")]
            public string state { get; set; }

        }

    }
}
