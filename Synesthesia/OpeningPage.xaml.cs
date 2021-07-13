using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Synesthesia
{
    public partial class OpeningPage : ContentPage
    {
        public OpeningPage()
        {
            InitializeComponent();
        }

        async void NextPageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ContinueButton.IsEnabled = false;

            await Navigation.PushAsync(new StartPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ContinueButton.IsEnabled = true;
        }
    }
}
