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
using System.Runtime.Serialization.Formatters;

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

            // Download the and save JSON to file.
            await GetJson();

            //Read data from JSON file
            var biddingSearchResults = ReadJson();

            // Update the live tile with the feed items.
            var tileUpdater = new TileUpdater();
            tileUpdater.UpdateTile(biddingSearchResults);
            

            // Inform the system that the task is finished.
            deferral.Complete();

        }

        //получаем ответ от JSON в виде строки и сохраняем в файл data.json
        //вынуждены использовать HttpBaseProtocolFilter для получения данных от не защищенного АПИ (отсувствует сертификат SSL)
        private static async Task GetJson()
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

                    var httpSerialize = await response.Content.ReadAsStringAsync();
                    var parsedString = Regex.Unescape(httpSerialize);
                    byte[] isoBites = Encoding.GetEncoding("ISO-8859-1").GetBytes(parsedString);
                    var jsonText = Encoding.UTF8.GetString(isoBites, 0, isoBites.Length);
                    if (jsonText != null)
                    {
                        try
                        {
                            File.Delete(PathFolder);
                        }
                        catch
                        {
                            int timeout = 5000;
                            Task deleteTask = Task.Run(() => File.Delete(PathFolder));
                            deleteTask.Wait(timeout);
                        }
                        File.WriteAllText(PathFolder, jsonText);
                    }
                }
            }

            catch (Exception ex)
            {                
                Debug.WriteLine(ex.ToString());
            }

        }
        
        // Проводим дессериализацию
        // Дессериализация выполняется по следующему примеру: 
        //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm#
        private static IList<Bidding> ReadJson()
        {
            var json = JObject.Parse(File.ReadAllText(PathFolder));
            // собираем JSON resultList objects в список объектов
            var resultList = json["result"].Children().ToList();
            
            var biddingSearchResults = new List<Bidding>();//результат работы цикла

            foreach (var res in resultList)
            {
                try
                {
                    var searchResult = res.ToObject<Bidding>();
                    if (searchResult.Title == null)
                    {continue;}                   
                    searchResult.EntityType = searchResult.EntityType.Equals("bid") ? "Заявка" : "аукцион\\редукцион";
                    biddingSearchResults.Add(searchResult);
                }
                catch (Exception) //на 3-й итерации цикла приходит пустой "owner", что вызывает ошибку
                {
                    continue;
                }
            }
            //случайная сортировка элеметов списка, согласно ТЗ
            for (int i = biddingSearchResults.Count - 1; i >= 1; i--)
            {
                var random = new Random();
                var j = random.Next(i + 1);     
                var temp = biddingSearchResults[j];
                biddingSearchResults[j] = biddingSearchResults[i];
                biddingSearchResults[i] = temp;
            }
            
            return biddingSearchResults;
        }
 
        static readonly StorageFolder GetLocalFolder = ApplicationData.Current.LocalFolder;
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.json"); //адрес файла в "title.xml" в системе

    }
}