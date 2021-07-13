using System.Linq;
using System.Collections.Generic;
using Synesthesia.Models;
using Xamarin.Forms;
using System;

namespace Synesthesia
{
    public partial class StartPage : ContentPage
    {
        private Dictionary<string, bool> Groups = new Dictionary<string, bool>();
        public StartPage()
        {
            InitializeComponent();

            Groups.Add("Letters", false);
            Groups.Add("Numbers", false);
            Groups.Add("DotW", false);
            Groups.Add("Months", false);

            GenerateUID();

            BindingContext = this;
        }

        async void StartTestButton_Clicked(System.Object sender, System.EventArgs e)
        {
            StartTestButton.IsEnabled = false;
            bool letters = Groups["Letters"];
            bool numbers = Groups["Numbers"];
            bool dow = Groups["DotW"];
            bool months = Groups["Months"];

            if(letters || numbers || dow || months)
            {
                await Navigation.PushAsync(new MainPage(letters, numbers, dow, months, UID.Text));
            } else 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Select at least one stimulus group", "OK");
                StartTestButton.IsEnabled = true;
            }
        }

        void Letters_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Groups["Letters"] = !Groups["Letters"];
        }

        void Numbers_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Groups["Numbers"] = !Groups["Numbers"];
        }

        void DotW_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Groups["DotW"] = !Groups["DotW"];
        }

        void Months_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Groups["Months"] = !Groups["Months"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartTestButton.IsEnabled = true;
        }

        private void GenerateUID()
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            UID.Text = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
