using PortableAssistant.Services;
using PortableAssistant.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortableAssistant
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<PersonsDataStore>();
            MainPage = new AppShell();
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
