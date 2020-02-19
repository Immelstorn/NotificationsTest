using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NotificationsTest.Services;
using NotificationsTest.Views;

namespace NotificationsTest
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            DependencyService.Get<INotificationManager>().Initialize();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
