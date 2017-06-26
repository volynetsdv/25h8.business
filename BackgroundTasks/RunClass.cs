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
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            if (jsonText != null)
            {
                var biddingSearchResults = new List<Bidding>();
                biddingSearchResults = LoadBidJson(jsonText);

                Save(biddingSearchResults);
            }
            //SaveData(jsonText);


            //Save();
            // Update the live tile with the feed items.
            TitleUpdater.UpdateTile();

            // Inform the system that the task is finished.
            deferral.Complete();

        }
        //получаем ответ от JSON в виде строки
        //вынуждены использовать HttpBaseProtocolFilter для получения данных от не защищенного АПИ (отсувствует сертификат SSL)
        private static async Task<string> GetFeed()
        {
            try
            {
                
                var filter = new HttpBaseProtocolFilter();
#if DEBUG
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
#endif
                using (var httpClient = new HttpClient(filter))
                {
                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(feedUrl));

                    string httpSerialize = await response.Content.ReadAsStringAsync();
                    string parsedString = Regex.Unescape(httpSerialize);
                    byte[] isoBites = Encoding.GetEncoding("ISO-8859-1").GetBytes(parsedString);
                    string jsonText = Encoding.UTF8.GetString(isoBites, 0, isoBites.Length);
                    if (jsonText != null)
                        return jsonText;
                   
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


        private static List<Bidding> LoadBidJson(string jsonText)
        {
            var json = JObject.Parse(jsonText);

            // собираем JSON resultList objects в список объектов
            var resultList = json["result"].Children().ToList();
            //результат работы цикла:
            var biddingSearchResults = new List<Bidding>();
           
            foreach (var res in resultList)
            {
                try
                {
                    var searchBidOwner = res.ToString();
                    var json1 = JObject.Parse(searchBidOwner);
                    var ownerList = json1["owner"].ToObject<Bidding>();
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    Bidding searchResult = res.ToObject<Bidding>();
                    searchResult.ContractorName = ownerList.ContractorName;
                    searchResult.LogoURL = ownerList.LogoURL;
                    biddingSearchResults.Add(searchResult);
                }
                catch (Exception) //на 3-й итерации цикла приходит пустой "owner", что вызывает ошибку
                {
                    var searchResult = res.ToObject<Bidding>();
                    searchResult.ContractorName = "Продавец не указан";
                    //searchResult.LogoURL = "";
                    biddingSearchResults.Add(searchResult);
                }

            }
            return biddingSearchResults;
        }
        // Проводим дессериализацию и сохраняем результат в файл
        // Дессериализация выполняется по следующему примеру: 
        //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm#

        private void Save(List<Bidding> biddingSearchResults) //должен получить объекты
        {
            var willSaveThisResult = biddingSearchResults;

            // пример результата из Newtonsoft
            // Title = <b>Paris Hilton</b> - Wikipedia, the free encyclopedia
            // Content = [1] In 2006, she released her debut album...
            // Url = http://en.wikipedia.org/wiki/Paris_Hilton

            //Проводим серриализацию полученных объектов в XML и сохраняем в файл

            XmlSerializer BidSaver = new XmlSerializer(typeof(Bid));
            XmlSerializer BiddingSaver = new XmlSerializer(typeof(Bidding));

            for (int i = 0; i < willSaveThisResult.Count; i++)
            {
                if (willSaveThisResult[i].Title != null)
                {
                    if (willSaveThisResult[i].EntityType.Contains("bid"))
                        using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                        {
                            BidSaver.Serialize(fs, willSaveThisResult[i]);
                        }
                    else
                        using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write)) //заменил OpenOrCreate на Append. Оставить если нет проблем с доступом к файлу
                        {
                            BiddingSaver.Serialize(fs, biddingSearchResults[i]);
                        }
                }
            }

        }

        //В метод нужно добавить перебор тайтлов,но для начала 
        //хочу добиться вывода на плитку хотя бы первого значения. Дальше все будет 
        //с реализацией сильно поможет статья для Вин8: https://habrahabr.ru/post/149219/


        // Although most HTTP servers do not require User-Agent header, others will reject the request or return
        // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
        //static string customHeaderName = "User-Agent";
        //static string customHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";


        static StorageFolder getLocalFolder = ApplicationData.Current.LocalFolder;
        static string path = Path.Combine(getLocalFolder.Path.ToString(), "title.xml"); //адрес файла в "title.xml" в системе

    }
    public sealed class Folder
    {


    }
}