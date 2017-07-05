using System;
using System.IO;
using System.Net.Http;
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
            RegisterBackgroundTask(TaskEntryPoint, TaskName);
            PinFirstTile();
        }

        //pin primary tile to Start if system can do it or primary tile existing
        private async void PinFirstTile()
        {
            // Get your own app list entry
            AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];
            // Check if Start supports your app
            bool isSupported = StartScreenManager.GetDefault().SupportsAppListEntry(entry);
            // Check if your app is currently pinned
            bool isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
        
        if (isPinned)
        {
            await GoToLanding();
        }

            // If tile isn't pinned and app can do it - send request to user
            if (isSupported)
            {
                // And pin it to Start
                var tileState = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
                if (tileState)
                {
                    DisplayResult.Text = "Плитка успешно закреплена в меню Пуск";
                    await Task.Delay(1900);
                    Application.Current.Exit();
                }
                //возможно этот блок можна убрать:
                else
                {
                    DisplayResult.Text = "Пожалуйста, закрепите плитку в меню Пуск";
                    await Task.Delay(2300);
                    await GoToLanding();
                }
            }
            // Send mesage if app can't pin tile to Start (and maybe if user has rejected request)
            else
            {
                //will have to testing:
                DisplayResult.Text = "Пожалуйста, закрепите плитку в меню Пуск";
                await GoToLanding();
            }

        }
        // for firstly tile updating ufter installetion
        private void FirstRun()
        {
            if (File.Exists(PathFolder) == false)
            {
                
                var jsonText = BackgroundTasks.RunClass.GetAndSaveJson();
                if (jsonText == null)
                {
                    RegisterBackgroundTask(TaskEntryPoint, TaskName);
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

        
        public async Task GoToLanding()
        {
            DisplayResult.Text = "Перенаправляю на сайт...";
            InternetChecker();

            await Task.Delay(2000);
            if (_internetChecker)
            {
                await Windows.System.Launcher.LaunchUriAsync(LandingUri);
                Application.Current.Exit();
            }
            else
            {
                DisplayResult.Text = "Проверьте соединение";
                await Task.Delay(2500);
                Application.Current.Exit();
            }
        }
        //Check Internet connection
        private bool InternetChecker()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = httpClient.GetAsync(new Uri(@"https://www.google.com.ua/"))
                        .GetAwaiter().GetResult();
                    return _internetChecker = true;
                }
                catch (Exception)
                {
                    return _internetChecker = false;
                }
            }
        }

        public Uri LandingUri = new Uri(@"https://stage.25h8.business/#!/landing");
        static readonly StorageFolder GetLocalFolder = ApplicationData.Current.LocalFolder;
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.json");
        private const string TaskName = "RunClass";
        private const string TaskEntryPoint = "BackgroundTasks.RunClass";
        private bool _internetChecker;


    }
}