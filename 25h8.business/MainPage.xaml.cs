using System;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace _25h8.business
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(300, 130));
            ApplicationView.PreferredLaunchViewSize = new Size(300, 130);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        //этот и ниже метод вместе с манифестом(раздел Объявления - точка входа) запускают фоновую службу. 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            FirstRun();
            RegisterBackgroundTask(taskEntryPoint, taskName);
            PinFirstTile();
        }

        public async Task<bool> GoToLanding()
        {
            DisplayResult.Text = "Перенаправляю на сайт...";
            await Task.Delay(1900);
            // Launch the URI            
            var success = await Windows.System.Launcher.LaunchUriAsync(LandingUri);
            return success;

        }
        //pin primary tile to Start if system can do it or primary tile existing
        private async void PinFirstTile()
        {
            // Get your own app list entry
            AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];
            // Check if Start supports your app
            bool isSupported = StartScreenManager.GetDefault().SupportsAppListEntry(entry);
            //проверяем закреплена ли плитка
            bool isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
        //если да - перейти на сайт

        if (isPinned)
        {
            var success = await GoToLanding();
            if (success)
            {
                // URI launched
            }
            else
            {
                DisplayResult.Text = "Проверьте соединение";
                await Task.Delay(1400);
                Application.Current.Exit();
                // URI launch failed
            }
                Application.Current.Exit();
        }
        //если нет и поддерживается програмное закрепление плитки - спросить пользователя
            if (isSupported)
            {
                // And pin it to Start
                //Обнаружил проблему при отключенном интернете. По не ясной мне причине 
                //програма не дожидается ответа пользователя и идет дальше по коду, что приводит 
                //к завершению программы до того, как пользователь поймет чего от него хотят
                bool pinnIfExist = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
               
                DisplayResult.Text = "Плитка успешно закреплена в меню Пуск";
                await Task.Delay(1900);
                Application.Current.Exit();
            }
            else
            {
                //требует тестирования:

                DisplayResult.Text = "Пожалуйста, закрепите плитку в меню Пуск";
                await Task.Delay(1500);
                DisplayResult.Text = "Перенаправляю на сайт";
                var success = await GoToLanding();
                if (success)
                {
                    // URI launched
                }
                else
                {
                    DisplayResult.Text = "Проверьте соединение";
                    await Task.Delay(1400);
                    Application.Current.Exit();
                    // URI launch failed
                }
                Application.Current.Exit();
            }

        }

        private void FirstRun()
        {
            if (File.Exists(PathFolder) == false)
            {
                
                var jsonText = BackgroundTasks.RunClass.GetAndSaveJson();
                if (jsonText == null)
                {
                    RegisterBackgroundTask(taskEntryPoint, taskName);
                    PinFirstTile();
                }
                else
                {
                    var biddingSearchResults = BackgroundTasks.RunClass.ReadJson(jsonText);

                    var tileUpdater = new BackgroundTasks.TileUpdater();
                    tileUpdater.UpdateTile(biddingSearchResults);
                }
            }
            
        }
        //
        // Register a background task with the specified taskEntryPoint, name, trigger
        //
        // taskEntryPoint: Task entry point for the background task.
        // taskName: A name for the background task.
        // trigger: The trigger for the background task.
        //
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
            string taskName)
        {
            //
            // Check for existing registrations of this background task.
            //

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {

                if (cur.Value.Name == taskName)
                {
                    //
                    // The task is already registered.
                    //

                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            //
            // Register the background task.
            //

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(new TimeTrigger(30, false));

            BackgroundTaskRegistration task = builder.Register();
            
            return task;
        }
        public Uri LandingUri = new Uri(@"https://stage.25h8.business/#!/landing");
        static readonly StorageFolder GetLocalFolder = ApplicationData.Current.LocalFolder;
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.json");
        private const string taskName = "RunClass";
        private const string taskEntryPoint = "BackgroundTasks.RunClass";
    }
}