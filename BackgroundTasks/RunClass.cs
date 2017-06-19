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
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BackgroundTasks
{
    public sealed class RunClass : IBackgroundTask
    {
            //сделано по образу и подобию из: https://docs.microsoft.com/ru-ru/windows/uwp/launch-resume/update-a-live-tile-from-a-background-task


            //мой Get не захватывает новую ссылку на API: 
            //static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";


        static string feedUrl = @"https://bankfund.sale/api/bidding?landing=true&limit=10&project=FG&state=in__completed,canceled,refused&way=auction";
        
        //здесь начинается выполнение фоновой задачи
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var jsonText = await GetFeed(); // получает значение от метода GetFeed и передает ниже в UpdateTile

            //save feed to XML
            Save_data(jsonText);

            // Update the live tile with the feed items.
            UpdateTile();

            // Inform the system that the task is finished.
            deferral.Complete();

        }
        //получаем ответ от JSON в виде строки
        private static async Task<string> GetFeed()
        {
            try
            {
                var client = new HttpClient();
                HttpResponseMessage jsonText = await client.GetAsync(new Uri(feedUrl));
                if (jsonText!=null)
                return await jsonText.Content.ReadAsStringAsync();
                else
                    return "";
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return "";
            }

        }

        // Проводим дессериализацию и сохраняем результат в файл
        // Дессериализация выполняется по следующему примеру: 
        //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm#
        private void Save_data(string jsonText)
        {
            JObject json = new JObject();
            json = JObject.Parse(jsonText);

            
            //string jsonText = @" ЗДЕСЬ ДОЛЖЕН БЫТЬ JSON текст";

            // get JSON result objects into a list
            IList<JToken> result = json["result"].ToList(); //<<<<< Ошибка. переменной присваивается null

            // Получаем две коллекции объектов (BID и BIDDING).
            // В результате операции исключаем из списка "result" не нужны полня
            // Вероятнее всего здесь ошибка и в один объект запихнет все данные, которые к нему относятся
            IList<BID> bidSearchResults = new List<BID>();
            foreach (JToken res in result)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                BID searchResult = res.ToObject<BID>();
                bidSearchResults.Add(searchResult);
            }


            IList<BIDDING> biddingSearchResults = new List<BIDDING>();
            foreach (JToken res in result)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                BIDDING searchResult = res.ToObject<BIDDING>();
                biddingSearchResults.Add(searchResult);
            }

                        // пример результата из Newtonsoft
                        // Title = <b>Paris Hilton</b> - Wikipedia, the free encyclopedia
                        // Content = [1] In 2006, she released her debut album...
                        // Url = http://en.wikipedia.org/wiki/Paris_Hilton

                        // Title = <b>Paris Hilton</b>
                        // Content = Self: Zoolander. Socialite <b>Paris Hilton</b>...
                        // Url = http://www.imdb.com/name/nm0385296/

            //Проводим серриализацию объектов полученных объектов в XML и сохраняем в файл
            XmlSerializer BID_saver = new XmlSerializer(typeof(BID));
            XmlSerializer BIDDING_saver = new XmlSerializer(typeof(BIDDING));

            for (int i = 0; i < bidSearchResults.Count; i++)
            {
                if (bidSearchResults[i].entityType.Contains(null))    // должно быть так if (bidSearchResults[i].entityType.Contains("bid")) 
                    using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                    {
                        BID_saver.Serialize(fs, bidSearchResults);
                    }
                else
                    using (FileStream fs = new FileStream("title.xml", FileMode.Append)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                    {
                        BIDDING_saver.Serialize(fs, biddingSearchResults);
                    }
            }

        }

        //В метод нужно добавить перебор тайтлов,но для начала 
        //хочу добиться вывода на плитку хотя бы первого значения. Дальше все будет 
        private static void UpdateTile()           
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification 
            
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);
            //здесь должен быть цикл
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