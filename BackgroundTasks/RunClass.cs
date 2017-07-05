using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Web.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json.Linq;
using Windows.Web.Http.Filters;
using Windows.Security.Cryptography.Certificates;
using System.Text.RegularExpressions;
using BackgroundTasks.Models;

namespace BackgroundTasks
{

    public sealed class RunClass : IBackgroundTask
    {
        //использован следующий пример: https://docs.microsoft.com/ru-ru/windows/uwp/launch-resume/update-a-live-tile-from-a-background-task

        static string feedUrl = @"https://stage.bankfund.sale/api/search?index=trade&limit=10&offset=0&populate=owner&project=MAIN";

        //здесь начинается выполнение фоновой задачи
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
          
            // Download the and save JSON to file.
            string jsonText = GetAndSaveJson();
         
            //Desserealize JSON 
            var biddingSearchResults = ReadJson(jsonText);
            if (biddingSearchResults == null)
            {
                deferral.Complete();
            }
            // Update the live tile with the feed items.
            else
            { 
            var tileUpdater = new TileUpdater();
            tileUpdater.UpdateTile(biddingSearchResults);
            }

            // Inform the system that the task is finished.
            deferral.Complete();

        }

        //получаем ответ от JSON в виде строки и сохраняем в файл data.json
        //вынуждены использовать HttpBaseProtocolFilter для получения данных от не защищенного АПИ (отсувствует сертификат SSL)
        public static string GetAndSaveJson()
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
                    HttpResponseMessage response = httpClient.GetAsync(new Uri(feedUrl)).GetAwaiter().GetResult();

                    var httpSerialize = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
                        return jsonText;
                    }
                }
            }
            
            catch (Exception)
            {
                return null;
            }
            return null; //это нужно, если ни try ни catche не отработали?
        }

        // Проводим дессериализацию
        public static IList<Bidding> ReadJson(string jsonText)
        {
            JObject json;
            if (File.Exists(PathFolder) == false)
            {
                return null; //если файла не существует - значит это первый запуск приложения и мы не смогли вытянуть данные с апи. Нет смысла продолжать. иначе - идем дальше 
            }
            else if (jsonText == null)
            {
                json = JObject.Parse(File.ReadAllText(PathFolder)); //мы уже знаем, что файл существует и если нам пришла пустая строка - значит нет соединения и нужно прочесть старые данные из файла. Иначе - идем дальше
            }
            else
            {
                json = JObject.Parse(jsonText); //если мы дошли сюда - мы знаем, что запрос был успешным и работаем с данными, которые нам передал предыдущий метод
            }
            // собираем JSON objects в список объектов resultList
            var resultList = json["result"].Children().ToList();

            var biddingSearchResults = new List<Bidding>();
            //цикл приводит список объектов из resultList в список объектов biddingSearchResults. Таким образом - мы получаем список объектов не содержащий лишних данных
            foreach (var res in resultList)
            {
                try
                {
                    var searchResult = res.ToObject<Bidding>();
                    if (searchResult.Title == null)
                    { continue; }
                    searchResult.EntityType = searchResult.EntityType.Equals("bid") ? "Заявка" : @"аукцион/редукцион";
                    biddingSearchResults.Add(searchResult);
                }
                catch (Exception) //на 3-й итерации цикла приходит пустой "owner", что вызывает ошибку. Пропускаем итерацию с ошибкой. Это нормально
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
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.json"); 

    }
}