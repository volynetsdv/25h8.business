////using Microsoft.Toolkit.Uwp.Notifications;
//using NotificationsExtensions.Tiles;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace BackgroundTasks
//{
//    class TestClass
//    {
//        //взято отсюда: https://github.com/WindowsNotifications/NotificationsExtensions/wiki/Tile-Notifications

//        static TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive()
//        {
//            PeekImage = new TilePeekImage()
//            {
//                Source = new TileImageSource("Assets/PeekImage.jpg").ToString()
//            },

//            Children =
//            {
//                new TileText()
//                {
//                    Text = "Notifications Extensions",
//                    Style = TileTextStyle.Body
//                },

//                new TileText()
//                {
//                    Text = "Generate notifications easily!",
//                    Wrap = true,
//                    Style = TileTextStyle.CaptionSubtle
//                }
//            }
//        };

//        TileBinding binding = new TileBinding()
//        {
//            Branding = TileBranding.NameAndLogo,

//            DisplayName = "Custom name",

//            Content = bindingContent
//        };


//        private static TileContent content = new TileContent()
//        {
//            Visual = new TileVisual()
//            {
//                TileMedium = binding,
//                TileWide = binding,
//                TileLarge = binding
//            }
//        };
//        XmlDocument doc = content.GetXml();

//        // Generate WinRT notification
//        new TileNotification(doc);

//    }
//    }
