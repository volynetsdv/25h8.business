﻿using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;


namespace BackgroundTasks
{
    public sealed class TileUpdater
    {
        static string textElementName = "title";
        static StorageFolder getLocalFolder = ApplicationData.Current.LocalFolder;
        static string path = Path.Combine(getLocalFolder.Path.ToString(), "title.xml"); //адрес файла в "title.xml" в системе

        public static void UpdateTile()
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification 
            //Для разных размеров плитки создадим  несколько таких строчек с "шаблонами тайлов":
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);
            //здесь должен быть цикл. [0] - это индекс элемента
            tileXml.GetElementsByTagName(textElementName)[0].InnerText = File.ReadAllText(path);

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

        TileContent content = new TileContent()
        {
            Visual = new TileVisual()
            {
                //TileMedium = ...

        TileWide = new TileBinding()
        {
            Content = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Jennifer Parker",
                        HintStyle = AdaptiveTextStyle.Subtitle
                    },

                    new AdaptiveText()
                    {
                        Text = "Photos from our trip",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },

                    new AdaptiveText()
                    {
                        Text = "Check out these awesome photos I took while in New Zealand!",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            }
        },

               // TileLarge = ...;
    }
        };
    }
}



//В метод нужно добавить перебор тайтлов,но для начала 
//хочу добиться вывода на плитку хотя бы первого значения. Дальше все будет 
//с реализацией сильно поможет статья для Вин8: https://habrahabr.ru/post/149219/


// Although most HTTP servers do not require User-Agent header, others will reject the request or return
// a different response if this header is missing. Use SetRequestHeader() to add custom headers.
//static string customHeaderName = "User-Agent";
//static string customHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";
