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
        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "proc")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public object Owner { get; set; }

        public Bid() { }

        public Bid(string entityType, string title, string process, int id, object owner)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            Id = id;
            Owner = owner;
        }
    }
    [Serializable]
    public sealed class BidOwner
    {
        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string LogoUrl { get; set; }

        public BidOwner() { }

        public BidOwner(string contractorName, string logoUrl)
        {
            ContractorName = contractorName;
            LogoUrl = logoUrl;
        }
    }
}