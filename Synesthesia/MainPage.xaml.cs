using System;
using System.Collections.Generic;
using Synesthesia.Models;
using System.Linq;

using Xamarin.Forms;

namespace Synesthesia
{
    public partial class MainPage : ContentPage
    {
        public string currentQuestionText { get; set; }
        private List<string> questions;

        private int questionCount = 0;

        private Dictionary<string, Color> colorAssociations;
        private Color DefaultColor;

        public MainPage(bool letters, bool numbers, bool dow, bool months)
        {
            InitializeComponent();

            questions = new List<string>();
            if (letters)
                questions.AddRange(Questions.Letters);
            if (numbers)
                questions.AddRange(Questions.Numbers);
            if (dow)
                questions.AddRange(Questions.DaysOfWeeks);
            if (months)
                questions.AddRange(Questions.Months);

            colorAssociations = new Dictionary<string, Color>();

            currentQuestionText = questions[questionCount];

            DefaultColor = Wheel.SelectedColor;

            BindingContext = this;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {

            Color color;
            if (Checkbox.IsChecked)
                color = Color.Transparent;
            else
                color = Wheel.SelectedColor;

            try
            {
                colorAssociations.Add(currentQuestionText, color);
            } catch (ArgumentException ex)
            {
                return;
            }
            

            if(questionCount == questions.Count - 1)
            {
                await Navigation.PushAsync(new ResultsPage(colorAssociations));
            } else
            {
                Wheel.SelectedColor = DefaultColor;

                if (questionCount == questions.Count - 2)
                    NextButton.Text = "Submit";

                questionCount++;
                currentQuestionText = questions[questionCount];
                ValueLabel.Text = questions[questionCount];
                Checkbox.IsChecked = false;
            }
        }

        void Checkbox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if(e.Value)
            {
                //checked
                Wheel.SelectedColor = DefaultColor;
                //await Wheel.FadeTo(0, 500);
                Wheel.IsEnabled = false;
            } else
            {
                //unchecked
                //await Wheel.FadeTo(1, 500);
                Wheel.IsEnabled = true;
            }
        }
    }
}
