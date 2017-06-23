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

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string LogoURL { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        public Bid() { }

        public Bid(string entityType, string title, string process, string contractorName, string logoURL, int id)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            ContractorName = contractorName;
            LogoURL = logoURL;
            Id = id;
        }
    }
}

//альтернатива, но был ряд проблем
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

//        [JsonProperty(PropertyName = "Id")]
//        public int Id { get; set; }

//        public BidOwner[] owner { get; set; }

//        public Bid() { }

//        public Bid(string entityType, string title, string process, string contractorName, string logoURL, int id)
//        {
//            EntityType = entityType;
//            Title = title;
//            Process = process;
//            Id = id;
//        }
//    }
//    [Serializable]
//    public sealed class BidOwner
//    {
//        [JsonProperty(PropertyName = "contractorName")]
//        public string ContractorName { get; set; }

//        [JsonProperty(PropertyName = "contractorAvatarUrl")]
//        public string LogoURL { get; set; }

//        public BidOwner() { }

//        public BidOwner(string contractorName, string logoURL)
//        {
//            ContractorName = contractorName;
//            LogoURL = logoURL;
//        }
//    }
//}