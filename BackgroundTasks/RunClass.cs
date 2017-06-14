using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
using Windows.System.Threading;

namespace BackgroundTasks
{
    public sealed class RunClass : IBackgroundTask
    {

        //слепил по образу и подобию из: https://docs.microsoft.com/ru-ru/windows/uwp/launch-resume/update-a-live-tile-from-a-background-task

        //Run практически не менял, но логику его понимаю не до конца. Вернее, не до конца 
        //понимаю, как в этом конкретном примере срабатывают асинхронные операции и не пытается ли методы
        //вызвать Save_feed(feed); не дождавшись завершения var feed = await GetFeed(); ?
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var feed = await GetFeed(); // получает значение от метода GetFeed и передает ниже в UpdateTile

            //save feed to XML
            Save_feed(feed); 

            // Update the live tile with the feed items.
            UpdateTile(); 

            // Inform the system that the task is finished.
            deferral.Complete();
            
        }

        private static async Task<string> GetFeed()
        {
            string feed = null;
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

        //отфильтровываем лишнее из GET запроса и сохраняем все в XML. И да... Если будет возникать ошибка доступа - нужно добавить соответствующий пункт в FileStream
        //Еще - пример асинхронной работы с файлами: https://social.msdn.microsoft.com/Forums/expression/en-US/6be36a29-d609-43b6-8c62-4be0dcf01f09/-threadpool-filestream?forum=programminglanguageru
        //Переделать в async, когда все заработает
        private static void Save_feed(string feed) 
        {
            var bid = JsonConvert.DeserializeObject<Deserialize.BID>(feed); //убираем все лишнее из ответа API
            XmlSerializer BID_saver = new XmlSerializer(typeof(Deserialize.BID)); 

            var bidding = JsonConvert.DeserializeObject<Deserialize.BIDDING>(feed);
            XmlSerializer BIDDING_saver = new XmlSerializer(typeof(Deserialize.BIDDING));

            if (bid.entityType == "bid")
                using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                {
                    BID_saver.Serialize(fs, bid);
                }
            else
                using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                {
                    BIDDING_saver.Serialize(fs, bidding);
                }
        }
        //входящим параметром был string feed. В метод нужно добавить перебор тайтлов,
        //но для начала хочу добиться вывода на плитку хотя бы первого значения
        private static void UpdateTile()           
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification 
            
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);

            tileXml.GetElementsByTagName(textElementName)[0].InnerText = File.ReadAllText("title.xml");

            // Create a new tile notification.
            if (tileXml != null)
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