using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            //Application.Current.Exit(); //команда завершения работы приложенияю В теории фоновая задача 
                                        //должна проходить регистрацию (процес на столько быстр, что без брикпоинтов пользователь 
                                        //практически ничего не заметит) и после - завершаться. Тем не менее отдельный фоновый процесс после регистрации
                                        //будет оставаться активным. Так же есть более эффективная альтернатива: https://docs.microsoft.com/en-us/uwp/api/Windows.ApplicationModel.Core.CoreApplication#Windows_ApplicationModel_Core_CoreApplication_EnteredBackground 

        }


        //этот и ниже метод вместе с манифестом(раздел Объявления - точка входа) запускают фоновую службу. 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RegisterBackgroundTask();
        }
        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(30, false));
                var registration = taskBuilder.Register();
            }

        }

        private const string taskName = "RunClass";
        private const string taskEntryPoint = "BackgroundTasks.RunClass";
    }
}