using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    internal class BID 
    { 
      
        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "proc")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "LogogURL")]
        public string LogogURL { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }
        

    }
}
