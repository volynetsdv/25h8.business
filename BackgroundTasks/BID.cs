using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class BID
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
}
