using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Synesthesia;

namespace Synesthesia
{
    public partial class OpeningPage : ContentPage
    {
        public double TextFontSize { get; set; }
        public double HeaderFontSize { get; set; }

        private String consent =
            "The purpose of this trial is to gather color data (R, G, and B values) to determine if the participant is synesthetic in all or some of the following categories: Letters, Numbers, Days of the Week, and Months. " +
            "The data collection will take place over 3 trials of each respective data set. The participant will be shown a stimulus and will respond with which color the participant associates with the stimulus. Participants will benefit by contributing to research that will better understand their condition and its implications. There are no known risks to engaging in this type of research." +
            "The nature of this research and the collection of data will help researchers better understand patterns and constants in synesthesia research. The generated data will not be identifiable. " +
            "If you would like to share your results with a researcher, screenshot your results when you have finished the test. The data collected could be stored indefinitely by whomever you send the screenshots to. " +
            "There will be no central data collection, so it is unknown how long the data collection phase will last. This will depend on the researcher who collected your data. If a participant would like to withdraw their data, please contact the individual who originally received your results. " +
            "If you have any questions about the application or confidentiality, do not hesitate to contact mrd5591@icloud.com.";

        private bool isFirstTime;

        public OpeningPage(bool isFirstTime)
        {
            InitializeComponent();

            this.isFirstTime = isFirstTime;
            TextFontSize = App.TextFontSize;
            HeaderFontSize = App.HeaderFontSize;

            BindingContext = this;
        }

        async void NextPageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ContinueButton.IsEnabled = false;

            await Navigation.PushAsync(new StartPage());
        }

        protected override async void OnAppearing()
        {
            if (isFirstTime)
            {
                bool result = await DisplayAlert("Health-Related Human Subject Consent", consent, "Accept", "Decline");
                if (!result)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                isFirstTime = false;
            }

            base.OnAppearing();

            ContinueButton.IsEnabled = true;
        }
    }
}
