using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Bid 
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

        public Bid() { }

        public Bid(string entityType, string title, string process, string contractorName, string logogURL, int id)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            ContractorName = contractorName;
            LogogURL = logogURL;
            Id = id;
        }


    }
}
