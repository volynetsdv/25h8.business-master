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

        public void UpdateTile(IList<Bidding> biddingSearchResults)
        {
            // Create a tile update manager 
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();  //судя по прочтенной информации - нам не нужно будет очищать плитку, но на всякий случай не удаляю          

            for (int i = 0; i < biddingSearchResults.Count; i++)
            {
                var title = biddingSearchResults[i].Title;
                var contractorName = biddingSearchResults[i].Owner.ContractorName;
                var logoURL = biddingSearchResults[i].Owner.LogoURL;
                var tipe = biddingSearchResults[i].EntityType;
                //этот код отправляет уведомление на политку используя содержимое из "content":
                var content = GetTileContent(title, contractorName, logoURL, tipe);
                var notification = new TileNotification(content.GetXml());
                updater.Update(notification);
            }
        }

        private TileContent GetTileContent(string title, string contractorName, string logoURL, string tipe)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    DisplayName = tipe, //вместо дня и даты задать ссылку на тип Bid Bidding (отобр. внизу слева)
                    TileSmall = new TileBinding()
                    {
                        Branding =
                            TileBranding
                                .Logo, //задаем подпись и лого в нижней части плитки.//Прописать для всех размеров
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title,
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
                        DisplayName = tipe,
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
                                Source = logoURL,
                                HintOverlay = 20
                            },
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },

                                new AdaptiveText()
                                {
                                    Text = contractorName,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle //Утонченный шрифт с 60% прозрачностью
                                }
                                //здесь добавляем текст так же как в TileWide
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        DisplayName = tipe,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = contractorName,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle //Утонченный шрифт с 60% прозрачностью
                                },

                                new AdaptiveText()
                                {
                                    Text = tipe,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                    //большая плитка
                    TileLarge = new TileBinding()
                    {
                        DisplayName = tipe,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup() {HintWeight = 1},
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 2,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = logoURL,
                                                    HintCrop = AdaptiveImageCrop.Circle
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup() {HintWeight = 1}
                                    }
                                },
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Title,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = contractorName,
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
}



//В метод нужно добавить перебор тайтлов,но для начала 
//хочу добиться вывода на плитку хотя бы первого значения. Дальше все будет 
//с реализацией сильно поможет статья для Вин8: https://habrahabr.ru/post/149219/


// Although most HTTP servers do not require User-Agent header, others will reject the request or return
// a different response if this header is missing. Use SetRequestHeader() to add custom headers.
//static string customHeaderName = "User-Agent";
//static string customHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:53.0) Gecko/20100101 Firefox/53.0";

