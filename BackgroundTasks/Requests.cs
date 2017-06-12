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
        static string feedUrl = @"https://bankfund.sale/api/bidding?landing=true&limit=10&project=FG&state=in__completed,canceled,refused&way=auction";

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
        public async void Runner()
        {
            await RunAsync();
        }
        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(feedUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                BID bid = new BID();
                BIDDING bidding = new BIDDING();
                // Get inf from API
                bid = await GetBIDAsync(feedUrl); 
                bidding = await GetBIDDINGAsync(feedUrl);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }

}