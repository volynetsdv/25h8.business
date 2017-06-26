using Newtonsoft.Json;
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
        //использован следующий пример: https://docs.microsoft.com/ru-ru/windows/uwp/launch-resume/update-a-live-tile-from-a-background-task

        static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";
        
        //здесь начинается выполнение фоновой задачи
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var jsonText = await GetFeed(); // получает значение от метода GetFeed и передает ниже в UpdateTile

            //save feed to XML
            if (jsonText != null)
            {
                var biddingSearchResults = new List<Bidding>();
                biddingSearchResults = LoadBidJson(jsonText);

                Save(biddingSearchResults);
            }
            
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
        
        // Проводим дессериализацию
        // Дессериализация выполняется по следующему примеру: 
        //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm#
        private static List<Bidding> LoadBidJson(string jsonText)
        {
            var json = JObject.Parse(jsonText);
            // собираем JSON resultList objects в список объектов
            var resultList = json["result"].Children().ToList();
            
            var biddingSearchResults = new List<Bidding>();//результат работы цикла

            foreach (var res in resultList)
            {
                try
                {
                    var searchBidOwner = res.ToString();
                    var json1 = JObject.Parse(searchBidOwner);
                    var ownerList = json1["owner"].ToObject<Owner>(); //практически такой же как resultList, но уже готовый к приведению к объекту
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    var searchResult = res.ToObject<Bidding>();
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

        // Cохраняем результат в файл
        private void Save(List<Bidding> biddingSearchResults) 
        {
            var willSaveThisResult = biddingSearchResults;

            //Проводим серриализацию полученных объектов в XML и сохраняем в файл

            XmlSerializer bidSaver = new XmlSerializer(typeof(Bid));
            XmlSerializer biddingSaver = new XmlSerializer(typeof(Bidding));

            for (int i = 0; i < willSaveThisResult.Count; i++)
            {
                if (willSaveThisResult[i].Title != null)
                {
                    if (willSaveThisResult[i].EntityType.Equals("bid"))
                        using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write)) //проблема с созданием слишком большого файла. Не знаю как чистить старые записи
                        {
                            bidSaver.Serialize(fs, willSaveThisResult[i]);
                        }
                    else
                        using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write)) //проблема с созданием слишком большого файла. Не знаю как чистить старые записи
                        {
                            biddingSaver.Serialize(fs, willSaveThisResult[i]);
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


        static readonly StorageFolder getLocalFolder = ApplicationData.Current.LocalFolder;
        static readonly string path = Path.Combine(getLocalFolder.Path, "title.xml"); //адрес файла в "title.xml" в системе

    }
}