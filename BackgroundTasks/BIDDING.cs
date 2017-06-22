using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Bidding
    {
        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "proc")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string LogoURL { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        public Bidding() { }

        public Bidding(string entityType, string title, string process, string contractorName, string logoURL, int id, string state)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            ContractorName = contractorName;
            LogoURL = logoURL;
            Id = id;
            State = state;
        }

    }
}
