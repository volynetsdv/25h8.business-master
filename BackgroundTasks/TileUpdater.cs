using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.Collections.Generic;

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
            updater.Clear();            

            for (int i = 0; i < biddingSearchResults.Count; i++)
            {
                var title = biddingSearchResults[i].Title;
                var contractorName = biddingSearchResults[i].Owner.ContractorName;
                var tipe = biddingSearchResults[i].EntityType;
                var ownerIcon = biddingSearchResults[i].Owner.OwnerIcon;
                var defaultOwnerIcon = @"Assets\owner-icon.png";
                if (ownerIcon == "")
                {
                    ownerIcon = defaultOwnerIcon;
                }
                var defaultBackground = @"Assets\DefaultTileBackground.png";
                var background = biddingSearchResults[i].Owner.BackgroundForTile;
                if (background == "")
                {
                    background = defaultBackground;
                }
                //этот код отправляет уведомление на политку используя содержимое из "content":
                var content = GetTileContent(title, contractorName, ownerIcon, tipe, background);
                var notification = new TileNotification(content.GetXml());
                updater.Update(notification);
            }
        }
        //данная структура задает правила отображения информации на плитках различных размеров:
        private TileContent GetTileContent(string title, string contractorName, string logoURL, string tipe, string background)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    //Есть два варианта средней плитки
                    //вариант №1:
                    DisplayName = tipe, 
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()//Проверить
                            {
                                Source = background
                            },
                            //PeekImage = new TilePeekImage()
                            //{
                            //    Source = logoURL,
                            //    HintOverlay = 20
                            //},
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintWrap = true,
                                },
                                new AdaptiveText()
                                {
                                    Text = contractorName,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                        //Вариант №2:
                        //DisplayName = tipe,
                        //Content = new TileBindingContentAdaptive()
                        //{
                        //    BackgroundImage = new TileBackgroundImage()//Проверить
                        //    {
                        //    Source = background
                        //},
                        //    Children =
                        //    {
                        //        new AdaptiveText()
                        //        {
                        //            Text = title,
                        //            HintWrap = true,
                        //            HintStyle = AdaptiveTextStyle.Base,
                        //            HintAlign = AdaptiveTextAlign.Center
                        //        }
                        //    }
                        //}
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = background
                            },
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        // Image column
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 33,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = logoURL,
                                                    HintCrop = AdaptiveImageCrop.Circle
                                                }
                                            }
                                        },
                                        // Text column
                                        new AdaptiveSubgroup()
                                        {
                                            // Vertical align its contents
                                            //TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = title,
                                                    HintWrap = true,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = contractorName,
                                                    HintWrap = true,
                                                    HintStyle = AdaptiveTextStyle.BodySubtle
                                                }
                                            }
                                        }
                                    }
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
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = background
                            },
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
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.Subtitle,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = contractorName,
                                    HintWrap = true,
                                    HintStyle = AdaptiveTextStyle.BaseSubtle,//SubtitleSubtle,
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




//с реализацией сильно поможет статья для Вин8: https://habrahabr.ru/post/149219/
