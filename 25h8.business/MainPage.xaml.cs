using System;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(200, 90));
            ApplicationView.PreferredLaunchViewSize = new Size(200, 90);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;



            //Application.Current.Exit(); //команда завершения работы приложенияю В теории фоновая задача 
            //должна проходить регистрацию (процес на столько быстр, что без брикпоинтов пользователь 
            //практически ничего не заметит) и после - завершаться. Тем не менее отдельный фоновый процесс после регистрации
            //будет оставаться активным. Так же есть более эффективная альтернатива: https://docs.microsoft.com/en-us/uwp/api/Windows.ApplicationModel.Core.CoreApplication#Windows_ApplicationModel_Core_CoreApplication_EnteredBackground 

        }


        //этот и ниже метод вместе с манифестом(раздел Объявления - точка входа) запускают фоновую службу. 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            FirstRun();
            RegisterBackgroundTask(taskEntryPoint, taskName);
            PinFirstTile();
        }
        //pin primary tile to Start if system can do it or primary tile existing
        private async void PinFirstTile()
        {
            // Get your own app list entry
            AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];
            // Check if Start supports your app
            bool isSupported = StartScreenManager.GetDefault().SupportsAppListEntry(entry);
            if (isSupported)
            {
                // And pin it to Start
                bool isPinned = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
            }

            await Task.Delay(1900);
            //эту часть еще нужно править, если получится
            // The URI to launch
            var baseUri = new Uri(@"https://stage.25h8.business/#!/landing");

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(baseUri);

            if (success)
            {
                // URI launched
            }
            else
            {
                Application.Current.Exit();
                // URI launch failed
            }
            Application.Current.Exit();
        }

        private void FirstRun()
        {
            if (File.Exists(PathFolder) == false)
            {
                //BackgroundTasks.RunClass.FirstGetJsonAsync();
                string jsonText = BackgroundTasks.RunClass.GetAndSaveJson();
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
        // Register a background task with the specified taskEntryPoint, name, trigger,
        // and condition (optional).
        //
        // taskEntryPoint: Task entry point for the background task.
        // taskName: A name for the background task.
        // trigger: The trigger for the background task.
        // condition: Optional parameter. A conditional event that must be true for the task to fire.
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
        static readonly StorageFolder GetLocalFolder = ApplicationData.Current.LocalFolder;
        static readonly string PathFolder = Path.Combine(GetLocalFolder.Path, "data.json");
        private const string taskName = "RunClass";
        private const string taskEntryPoint = "BackgroundTasks.RunClass";
    }
}