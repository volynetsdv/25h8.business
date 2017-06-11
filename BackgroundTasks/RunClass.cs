using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace BackgroundTasks
{
    public sealed class RunClass : IBackgroundTask
    {

        public static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var feed = await GetFeed();

            // Update the live tile with the feed items.
            UpdateTile(feed);

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
        private static async Task<XmlDocument> GetFeed()
        {
            //string feed = null;
            XmlDocument feed = new XmlDocument();
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(new Uri(@"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN"));
                var result = await response.Content.ReadAsStringAsync();
                feed.LoadXml(result);
                // XNode node = JsonConvert.DeserializeXNode(result, "title"); //http://www.newtonsoft.com/json/help/html/ConvertJsonToXml.htm#!

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return feed;
        }

        private static void UpdateTile(XmlDocument feed)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Keep track of the number feed items that get tile notifications.
            //int itemCount = 0;

            // Create a tile notification for each feed item.

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);

            //var title = feed;
            //string titleText = title == null ? String.Empty : title;
            tileXml.GetElementsByTagName(textElementName)[0].InnerText = feed.ToString();

            // Create a new tile notification.
            updater.Update(new TileNotification(tileXml));

            // Don't create more than 5 notifications.
            //if (itemCount++ > 10) break;

        }
        //public static async Task UpdateTiles(string xml)
        //{
        //    XmlDocument doc;

        //    try
        //    {
        //        doc = new XmlDocument();
        //        doc.LoadXml(xml);
        //    }

        //    catch (Exception ex)
        //    {
        //        await new MessageDialog(ex.ToString(), "ERROR: Invalid XML").ShowAsync();
        //        return;
        //    }

        //    await UpdateTiles(doc);
        //}

        // Although most HTTP servers do not require User-Agent header, others will reject the request or return
        // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
        //static string customHeaderName = "User-Agent";
        //static string customHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";

        static string textElementName = "text";
    }
}