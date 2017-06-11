using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Diagnostics;

namespace BackgroundTasks
{
    public sealed class Requests
    {
        static HttpClient client = new HttpClient();
        //Request inf for BID
        static async Task<BID> GetBIDAsync(string feedUrl)
        {
            BID bid = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bid = await response.Content.ReadAsAsync<BID>();
            }
            return bid;
        }
        //Request inf for BIDDING
        static async Task<BIDDING> GetBIDDINGAsync(string feedUrl)
        {
            BIDDING bidding = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bidding = await response.Content.ReadAsAsync<BIDDING>();
            }
            return bidding;
        }
        public async static void Runner()
        {
            await RunAsync();
        }
        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(BackgroundTasks.RunClass.feedUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                BID bid = new BID();
                BIDDING bidding = new BIDDING();
                // Get inf from API
                bid = await GetBIDAsync(BackgroundTasks.RunClass.feedUrl);
                bidding = await GetBIDDINGAsync(BackgroundTasks.RunClass.feedUrl);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }
    public sealed class BID
    {
        public string title { get; set; }
        public string proc { get; set; }
        public string contractorName { get; set; }
        public string LogogURL { get; set; }
        public int Id { get; set; }
    }
    public sealed class BIDDING
    {
        public string title { get; set; }
        public string proc { get; set; }
        public string contractorName { get; set; }
        public string PublicURL { get; set; }
        public string LogogURL { get; set; }
        public int Id { get; set; }
        public string state { get; set; }

    }
}