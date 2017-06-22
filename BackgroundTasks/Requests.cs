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
        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(feedUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Bid bid = new Bid();
                Bidding bidding = new Bidding();
                // Get inf from API
                bid = await GetBIDAsync(feedUrl);
                bidding = await GetBIDDINGAsync(feedUrl);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        //Request inf for BID
        static async Task<Bid> GetBIDAsync(string feedUrl)
        {
            Bid bid = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bid = await response.Content.ReadAsAsync<Bid>();
            }
            return bid;
        }
        //Request inf for BIDDING
        static async Task<Bidding> GetBIDDINGAsync(string feedUrl)
        {
            Bidding bidding = null;
            HttpResponseMessage response = await client.GetAsync(feedUrl);
            if (response.IsSuccessStatusCode)
            {
                bidding = await response.Content.ReadAsAsync<Bidding>();
            }
            return bidding;
        }



        //Класс для поиска:

        //public class SearchResult

        //   public string Title { get; set; }
        //   public string Content { get; set; }
        //   public string Url { get; set; }



        //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm#
        //string jsonText = @" ЗДЕСЬ ДОЛЖЕН БЫТЬ JSON текст";

        //JObject json = JObject.Parse(jsonText);

        //// get JSON result objects into a list
        //IList<JToken> results = json["responseData"]["results"].Children().ToList();

        //// serialize JSON results into .NET objects
        //IList<SearchResult> searchResults = new List<SearchResult>();
        //        foreach (JToken result in results)
        //        {
        //            // JToken.ToObject is a helper method that uses JsonSerializer internally
        //            SearchResult searchResult = result.ToObject<SearchResult>();
        //            searchResults.Add(searchResult);
        //        }

    //      Эти данные были внутри jsonText
    //    // Title = <b>Paris Hilton</b> - Wikipedia, the free encyclopedia
    //    // Content = [1] In 2006, she released her debut album...
    //    // Url = http://en.wikipedia.org/wiki/Paris_Hilton

    //    // Title = <b>Paris Hilton</b>
    //    // Content = Self: Zoolander. Socialite <b>Paris Hilton</b>...
    //    // Url = http://www.imdb.com/name/nm0385296/
















}

}