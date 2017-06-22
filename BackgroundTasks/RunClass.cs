﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using Windows.Web.Http;
//using System.Net.Http;
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
using System.Security.Cryptography.X509Certificates;
using Windows.Web.Http.Filters;
using Windows.Security.Cryptography.Certificates;

namespace BackgroundTasks
{
    public sealed class RunClass : IBackgroundTask
    {

        //сделано по образу и подобию из: https://docs.microsoft.com/ru-ru/windows/uwp/launch-resume/update-a-live-tile-from-a-background-task


            //мой Get не захватывает новую ссылку на API: 
            static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";


        //static string feedUrl = @"https://bankfund.sale/api/bidding?landing=true&limit=10&project=FG&state=in__completed,canceled,refused&way=auction";
        
        //здесь начинается выполнение фоновой задачи
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            StorageFolder localFolder = ApplicationData.Current.LocalFolder; //получаем текущую папку доступную для записи(локальная папка приложения)
            StorageFile newFile = await localFolder.CreateFileAsync("title.xml", CreationCollisionOption.OpenIfExists); //создаем файл в єтой папке
            

            // Download the feed.
            var jsonText = await GetFeed(); // получает значение от метода GetFeed и передает ниже в UpdateTile

            //save feed to XML or end the task
            if (jsonText!=null)
                SaveData(jsonText);
            else
                deferral.Complete();

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
                //var client = new HttpClient();
                var filter = new HttpBaseProtocolFilter();
#if DEBUG
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
#endif
                using (var httpClient = new HttpClient(filter))
                {
                    HttpResponseMessage jsonText = await httpClient.GetAsync(new Uri(feedUrl));

                    if (jsonText != null)
                        return await jsonText.Content.ReadAsStringAsync();
                    else
                        return null;
                }
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
        private void SaveData(string jsonText)
        {
            //StorageFile openFile = await localFolder.GetFileAsync("title.xml");
            //var stream = await newFile.OpenAsync(FileAccessMode.ReadWrite); //открываем файл для работы с ним

            //string jsonText = @" ЗДЕСЬ ДОЛЖЕН БЫТЬ JSON текст";
            JObject json = new JObject();
            json = JObject.Parse(jsonText);

            // get JSON result objects into a list
            IList<JToken> result = json["result"].ToList(); //<<<<< Ошибка. переменной присваивается null

            // Получаем две коллекции объектов (BID и BIDDING).
            // В результате операции исключаем из списка "result" не нужны полня
            // Вопрос: не запихнет ли оно в один объект все что у нас есть?
            IList<Bid> bidSearchResults = new List<Bid>();
            foreach (JToken res in result)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Bid searchResult = res.ToObject<Bid>();
                bidSearchResults.Add(searchResult);
            }


            IList<Bidding> biddingSearchResults = new List<Bidding>();
            foreach (JToken res in result)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Bidding searchResult = res.ToObject<Bidding>();
                biddingSearchResults.Add(searchResult);
            }

            // пример результата из Newtonsoft
            // Title = <b>Paris Hilton</b> - Wikipedia, the free encyclopedia
            // Content = [1] In 2006, she released her debut album...
            // Url = http://en.wikipedia.org/wiki/Paris_Hilton

            //Проводим серриализацию объектов полученных объектов в XML и сохраняем в файл

            XmlSerializer BidSaver = new XmlSerializer(typeof(Bid));
            XmlSerializer BiddingSaver = new XmlSerializer(typeof(Bidding));
            
            var getLocalFolder = ApplicationData.Current.LocalFolder;
            var path = Path.Combine(getLocalFolder.Path.ToString(), "title.xml");

            for (int i = 0; i < bidSearchResults.Count; i++)
            {
                if (bidSearchResults[i].EntityType.Contains("bid"))

                    //{
                    //    //по инструкции: Writing text to a file by using a stream (4 steps)
                    //    //https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-reading-and-writing-files
                    //    using (var outputStream = stream.GetOutputStreamAt(0))
                    //    {
                    //        using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    //        {
                    //            dataWriter.WriteString();
                    //            await dataWriter.StoreAsync();
                    //            await outputStream.FlushAsync();
                    //        }

                    //    }
                    //    stream.Dispose();
                    //}
                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                    {
                        BidSaver.Serialize(fs, bidSearchResults[i]);
                    }
                else
                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                    {
                        BiddingSaver.Serialize(fs, biddingSearchResults[i]);
                    }
            }

        }

        //В метод нужно добавить перебор тайтлов,но для начала 
        //хочу добиться вывода на плитку хотя бы первого значения. Дальше все будет 
        //с реализацией сильно поможет статья для Вин8: https://habrahabr.ru/post/149219/
        private static void UpdateTile()           
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification 
            //Для разных размеров плитки создадим  несколько таких строчек с "шаблонами тайлов":
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);
            //здесь должен быть цикл. [0] - это индекс элемента
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