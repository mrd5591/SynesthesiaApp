using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Synesthesia
{
    public partial class ResultsListPage : ContentPage
    {
        public Dictionary<string, List<Color>> colorMapping { get; set; }
        private string Username;
        private double score;

        public ResultsListPage(Dictionary<string, List<Color>> colorMapping, string Username)
        {
            InitializeComponent();

            this.colorMapping = colorMapping;
            this.Username = Username;

            ComputeResults();

            BindingContext = this;

            int i = 0;
            foreach(var item in ResultListView.Children)
            {
                if (i % 2 == 1)
                    ((Grid)item).BackgroundColor = Color.White;
                else
                    ((Grid)item).BackgroundColor = Color.White;

                i++;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NextButton.IsEnabled = true;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            NextButton.IsEnabled = false;

            await Navigation.PushAsync(new ResultsPage(colorMapping, Username, score));
        }

        private void ComputeResults()
        {
            int N = colorMapping.Count;

            double sum = 0;

            foreach (KeyValuePair<string, List<Color>> pair in colorMapping)
            {
                List<Color> val = pair.Value;

                if (val[0].Equals(Color.Transparent) && val[1].Equals(Color.Transparent) && val[2].Equals(Color.Transparent))
                {
                    N--;
                    continue;
                }

                sum += Math.Abs(val[0].R - val[1].R);
                sum += Math.Abs(val[0].G - val[1].G);
                sum += Math.Abs(val[0].B - val[1].B);

                sum += Math.Abs(val[0].R - val[2].R);
                sum += Math.Abs(val[0].G - val[2].G);
                sum += Math.Abs(val[0].B - val[2].B);

                sum += Math.Abs(val[1].R - val[2].R);
                sum += Math.Abs(val[1].G - val[2].G);
                sum += Math.Abs(val[1].B - val[2].B);
            }

            score = sum / N;
            result.Text = "Score = " + Math.Round(score, 7);
        }
    }
}
