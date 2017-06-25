//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BackgroundTasks
//{

//    public class Rootobject
//    {
//        public Result[] result { get; set; }
//    }

//    public class Result
//    {
//        public int id { get; set; }
//        public string identity { get; set; }
//        public string way { get; set; }
//        public string proc { get; set; }
//        public string state { get; set; }
//        public string title { get; set; }


//        public string rejectFileName { get; set; }
//        public string rejectFileUrl { get; set; }

//        public string publications { get; set; }
//        public string customerName { get; set; }


//        public int applicantAttemptsMax { get; set; }

//        public string entityType { get; set; }
//        public object owner { get; set; }

//        public string currency { get; set; }
//        public string currencyName { get; set; }

//        public string biddingProc { get; set; }

//    }


//    public class Protocoldata
//    {
//        public Bidding bidding { get; set; }
//    }

//    public class Bidding
//    {
//        public int id { get; set; }
//        public string proc { get; set; }
//        public string state { get; set; }
//        public string title { get; set; }

//        public Member[] members { get; set; }
//        public string currency { get; set; }
//        public string publicLink { get; set; }
//    }

//    public class Member
//    {
//        public Files1 files { get; set; }
//    }

//    public class Files1
//    {
//        public object[] result { get; set; }
//    }

//    public class Esdata
//    {
//        public string Type { get; set; }
//        public string Id { get; set; }
//        public float Score { get; set; }
//        public Source Source { get; set; }
//    }

//    public class Source
//    {
//        public string entityType { get; set; }
//        public string state { get; set; }
//    }

//}
