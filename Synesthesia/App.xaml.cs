using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Synesthesia
{
    public partial class App : Application
    {
        public static string url = "https://synesthesia-315922.uk.r.appspot.com/";
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage());
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
