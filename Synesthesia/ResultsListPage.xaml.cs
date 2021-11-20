using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Synesthesia.Models;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Synesthesia
{
    public partial class ResultsListPage : ContentPage
    {
        public List<StimulusResult> colorMapping { get; set; }
        private string Username;
        public double overallScore { get; set; }

        public List<StimulusGroup> StimulusGroups { get; set; }
        public ResultListViewModel vm { get; set; }

        public ResultsListPage(List<StimulusResult> _colorMapping/*, string Username*/, bool letters, bool numbers, bool dotw, bool months)
        {
            InitializeComponent();

            this.colorMapping = new List<StimulusResult>();
            StimulusGroups = new List<StimulusGroup>();

            if (letters)
            {
                StimulusGroup letterGroup = new StimulusGroup("Letters");
                StimulusGroups.Add(letterGroup);

                foreach(string stim in Questions.Letters)
                {
                    StimulusResult res = _colorMapping.FirstOrDefault(X => X.Name.Equals(stim));
                    if (res != null)
                    {
                        colorMapping.Add(res);
                        letterGroup.Stimuli.Add(res);
                    }  
                }
            }
            if (numbers)
            {
                StimulusGroup numberGroup = new StimulusGroup("Numbers");
                StimulusGroups.Add(numberGroup);

                foreach (string stim in Questions.Numbers)
                {
                    StimulusResult res = _colorMapping.FirstOrDefault(X => X.Name.Equals(stim));
                    if (res != null)
                    {
                        colorMapping.Add(res);
                        numberGroup.Stimuli.Add(res);
                    }
                }
            }
            if (dotw)
            {
                StimulusGroup dotwGroup = new StimulusGroup("Days of the Week");
                StimulusGroups.Add(dotwGroup);

                foreach (string stim in Questions.DaysOfWeeks)
                {
                    StimulusResult res = _colorMapping.FirstOrDefault(X => X.Name.Equals(stim));
                    if (res != null)
                    {
                        colorMapping.Add(res);
                        dotwGroup.Stimuli.Add(res);
                    }
                }
            }
            if (months)
            {
                StimulusGroup monthGroup = new StimulusGroup("Months");
                StimulusGroups.Add(monthGroup);

                foreach (string stim in Questions.Months)
                {
                    StimulusResult res = _colorMapping.FirstOrDefault(X => X.Name.Equals(stim));
                    if (res != null)
                    {
                        colorMapping.Add(res);
                        monthGroup.Stimuli.Add(res);
                    }
                }
            }

            //this.Username = Username;

            vm = new ResultListViewModel();
            vm.Groups = StimulusGroups;

            ComputeResults(StimulusGroups);

            if (App.DeviceIdiom == DeviceIdiom.Tablet)
                vm.Marg = new Thickness(App.WidthConstant / 15, 0);
            else
                vm.Marg = new Thickness(0);

            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NextButton.IsEnabled = true;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            NextButton.IsEnabled = false;

            await Navigation.PushAsync(new ResultsPage(StimulusGroups, /*Username,*/ overallScore));
        }

        private void ComputeResults(List<StimulusGroup> StimulusGroups)
        {
            double overallSum = 0;
            int overallN = colorMapping.Count;

            foreach(StimulusGroup group in StimulusGroups)
            {
                int N = group.Stimuli.Count;

                double sum = 0;

                foreach (StimulusResult res in group.Stimuli)
                {
                    List<Color> val = res.Colors;

                    if (val[0].Equals(Color.Transparent) && val[1].Equals(Color.Transparent) && val[2].Equals(Color.Transparent))
                    {
                        N--;
                        overallN--;
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

                overallSum += sum;

                group.Score = sum / N;
            }

            overallScore = overallSum / overallN;

            result.Text = "Score = " + Math.Round(StimulusGroups[0].Score, 2);
        }

        void CarouselStimView_Scrolled(System.Object sender, Xamarin.Forms.ItemsViewScrolledEventArgs e)
        {
            result.Text = "Score = " + Math.Round(StimulusGroups[e.CenterItemIndex].Score, 2);
        }
    }
}
