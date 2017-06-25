//using Newtonsoft.Json;
//using System;

//namespace BackgroundTasks
//{
//    [Serializable]
//    public sealed class Bid
//    {
//        [JsonProperty(PropertyName = "entityType")]
//        public string EntityType { get; set; }

//        [JsonProperty(PropertyName = "title")]
//        public string Title { get; set; }

//        [JsonProperty(PropertyName = "proc")]
//        public string Process { get; set; }

//        [JsonProperty(PropertyName = "contractorName")]
//        public string ContractorName { get; set; }

//        [JsonProperty(PropertyName = "contractorAvatarUrl")]
//        public string LogoUrl { get; set; }

//        [JsonProperty(PropertyName = "Id")]
//        public int Id { get; set; }

//        public Bid() { }

//        public Bid(string entityType, string title, string process, string contractorName, string logoUrl, int id)
//        {
//            EntityType = entityType;
//            Title = title;
//            Process = process;
//            ContractorName = contractorName;
//            LogoUrl = logoUrl;
//            Id = id;
//        }
//    }
//}

//альтернатива, но был ряд проблем
using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Bid
    {
        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string LogoURL { get; set; }

        public Bid() { }

        public Bid(string contractorName, string logoURL)
        {
            ContractorName = contractorName;
            LogoURL = logoURL;

        }
    }
    
}