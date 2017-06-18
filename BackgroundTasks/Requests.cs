using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Diagnostics;



namespace BackgroundTasks
{   
    //Воторой вариант получения данных от АПИ. Слепил из примеров на MSDN
    public sealed class Requests
    {
        static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";

        static HttpClient client = new HttpClient();
        //Request inf for BID
        static async Task<Deserialize.BID> GetBIDAsync(string feedUrl)
        {
            Deserialize.BID bid = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bid = await response.Content.ReadAsAsync<Deserialize.BID>();
            }
            return bid;
        }
        //Request inf for BIDDING
        static async Task<Deserialize.BIDDING> GetBIDDINGAsync(string feedUrl)
        {
            Deserialize.BIDDING bidding = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bidding = await response.Content.ReadAsAsync<Deserialize.BIDDING>();
            }
            return bidding;
        }

        public static async Task<string> RunAsync()
        {
            client.BaseAddress = new Uri(feedUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string feed = null;
            try
            {
                Deserialize.BID bid = new Deserialize.BID();
                Deserialize.BIDDING bidding = new Deserialize.BIDDING();
                // Get inf from API
                bid = await GetBIDAsync(feedUrl); 
                bidding = await GetBIDDINGAsync(feedUrl);
                if (bid.entityType == "bid")
                {
                    feed = bid.ToString();
                    return feed;
                }
                else
                {
                    feed = bidding.ToString();
                    return feed;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return feed;
        }

    }

}