using System;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using System.Collections.Generic;
//using NotificationsExtensions.Tiles;

namespace BackgroundTasks
{
    public sealed class TileUpdater
    {
        //Полезные ссылки:
        //настройка внешнего вида: https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/06/30/adaptive-tile-templates-schema-and-documentation/
        //Отправка локального уведомления на плитку: https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/10/05/quickstart-sending-a-local-tile-notification-in-windows-10/
        //Все вместе:https://github.com/WindowsNotifications/NotificationsExtensions/wiki/Tile-Notifications
        //но ничего не выходит
        
        static string textElementName = "title";
        static readonly StorageFolder GetLocalFolder = ApplicationData.Current.LocalFolder;
        
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.xml"); //адрес файла в "title.xml" в системе
        
        public void UpdateTile(IList<Bidding> biddingSearchResults)
        {
            
            // Create a tile update manager
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            //updater.Clear();  //судя по прочтенной информации - нам не нужно будет очищать плитку, но на всякий случай не удаляю          

            //этот код должен отправлять уведомление на политку:
            var notification = new TileNotification(content.GetXml()); //какая нафиг ссылка на обыект? А  content это не объект? Я его даже паблик сделал
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

            //Не могу понять, как работать с странным выражением ниже, 
            //которое начинается вот так: TileContent content = new TileContent())

            //___старый кусок кода________________________________________________________________
            //// Create a tile notification 
            ////Для разных размеров плитки создадим  несколько таких строчек с "шаблонами тайлов":
            //XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);
            ////здесь должен быть цикл. [0] - это индекс элемента
            //tileXml.GetElementsByTagName(textElementName)[0].InnerText = File.ReadAllText(PathFolder);

            //// Create a new tile notification.
            //if (tileXml != null)
            //{
            //    updater.Update(new TileNotification(tileXml));
            //}
            //else
            //    Debug.WriteLine(":((("); ;
            //// Don't create more than 5 notifications.
            ////if (itemCount++ > 10) break;
            //____________________________________________________________________________________
        }


        private TileContent content = new TileContent()
        {
            Visual = new TileVisual()
            {
                Branding = TileBranding.Logo,
                //DisplayName = "Wednesday 22", //вместо дня и даты задать ссылку на тип Bid Bidding (отобр. внизу слева)
                TileSmall = new TileBinding()
                {
                    //{		//задаем подпись и лого в нижней части плитки. 
                    //Прописать для всех размеров
                    //Branding = TileBranding.Logo,
                    //...
                    //},
                    Content = new TileBindingContentAdaptive()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Small",
                                HintWrap = true,
                                HintStyle = AdaptiveTextStyle.Base,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                            //HintWrap отвечает за перенос текста по словам в плитке и его стиль
                            //HintAlign - выравнивание текста
                            //здесь добавляем текст так же как в TileWide(ниже)
                        }
                    }
                },
                //Branding = TileBranding.NameAndLogo,
                TileMedium = new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        //фоновое изображение:
                        BackgroundImage = new TileBackgroundImage()
                        {
                            Source = "Assets/Mostly Cloudy-Background.jpg"
                        },
                        //изображение обновляемое вместе с уведомлением. 
                        PeekImage = new TilePeekImage()
                        {
                            Source = "Assets/Map.jpg",
                            HintOverlay = 20
                        },
                        Children =
                        {
                            new AdaptiveText() {Text = "title",
                                HintWrap = true,
                                HintStyle = AdaptiveTextStyle.Base,
                                HintAlign = AdaptiveTextAlign.Center}
                            //здесь добавляем текст так же как в TileWide
                        }
                    }
                },

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
                                HintStyle = AdaptiveTextStyle.CaptionSubtle //Утонченный шрифт с 60% прозрачностью
                            },

                            new AdaptiveText()
                            {
                                Text = "Check out these awesome photos I took while in New Zealand!",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                            }
                        }
                    }
                },
                //большая плитка
                TileLarge = new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        TextStacking = TileTextStacking.Center,
                        Children =
                        {
                            new AdaptiveGroup()
                            {
                                Children =
                                {
                                    new AdaptiveSubgroup() { HintWeight = 1 },
                                    new AdaptiveSubgroup()
                                    {
                                        HintWeight = 2,
                                        Children =
                                        {
                                            new AdaptiveImage()
                                            {
                                                Source = "Assets/Apps/Hipstame/hipster.jpg",
                                                HintCrop = AdaptiveImageCrop.Circle
                                            }
                                        }
                                    },
                                    new AdaptiveSubgroup() { HintWeight = 1 }
                                }
                            },
                            new AdaptiveText()
                            {
                                Text = "Hi,",
                                HintStyle = AdaptiveTextStyle.Title,
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveText()
                            {
                                Text = "MasterHip",
                                HintStyle = AdaptiveTextStyle.SubtitleSubtle,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    }
                }
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

