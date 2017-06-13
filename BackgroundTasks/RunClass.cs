using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace BackgroundTasks
{
    public sealed class RunClass : IBackgroundTask
    {

        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var feed = await GetFeed(); // получает значение от метода GetFeed и передает ниже в UpdateTile

            // Update the live tile with the feed items.
            UpdateTile(feed); //принимает значение от GetFeed

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        //private static async Task<SyndicationFeed> GetFeed()
        //{
        //    SyndicationFeed feed = null;

        //    try
        //    {
        //        // Create a syndication client that downloads the feed.  
        //        SyndicationClient client = new SyndicationClient();
        //        client.BypassCacheOnRetrieve = true;
        //        client.SetRequestHeader(customHeaderName, customHeaderValue);

        //        // Download the feed.
        //        feed = await client.RetrieveFeedAsync(new Uri(feedUrl));
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //    }

        //    return feed;
        //}

        private static async Task<string> GetFeed()
        {
            string feed = null;
            //XmlDocument feed = new XmlDocument();
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(new Uri(@"https://bankfund.sale/api/bidding?landing=true&limit=10&project=FG&state=in__completed,canceled,refused&way=auction"));
                if (response.IsSuccessStatusCode)
                {
                    feed = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return feed;
        }

        //отфильтровываем лишнее из GET запроса и сохраняем все в XML
        private static void Save_feed(string feed) 
        {
            var BID = JsonConvert.DeserializeObject<Deserialize.BID>(feed); //убираем все лишнее из ответа API
            XmlSerializer BID_saver = new XmlSerializer(typeof(Deserialize.BID)); 

            var BIDDING = JsonConvert.DeserializeObject<Deserialize.BIDDING>(feed);
            XmlSerializer BIDDING_saver = new XmlSerializer(typeof(Deserialize.BIDDING));

            if (BID.entityType == "bid")
                using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                {
                    BID_saver.Serialize(fs, BID);
                }
            else
                using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                {
                    BIDDING_saver.Serialize(fs, BIDDING);
                }
        }

        private static void UpdateTile() //будет переделан, когда разберусь с сохранением. входящим аргументом был string feed
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Keep track of the number feed items that get tile notifications.
            //int itemCount = 0;

            // Create a tile notification for each feed item.
            StreamReader rd_file = new StreamReader("title.xml");// здесь должен быть адрес файла из строки 97
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);

            //var title = feed;
            //string titleText = title == null ? String.Empty : title;
            tileXml.GetElementsByTagName(textElementName)[0].InnerText = feed.ToString();

            // Create a new tile notification.
            if (feed != null)
            {
                updater.Update(new TileNotification(tileXml));
            }
            else
                Debug.WriteLine(":((("); ;
            // Don't create more than 5 notifications.
            //if (itemCount++ > 10) break;

        }

        // Although most HTTP servers do not require User-Agent header, others will reject the request or return
        // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
        //static string customHeaderName = "User-Agent";
        //static string customHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";

        static string textElementName = "title";


    }
}