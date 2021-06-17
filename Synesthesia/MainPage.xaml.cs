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

        private List<string> bucket1;
        private List<string> bucket2;
        private List<string> bucket3;

        private int questionCount = 0;

        private Dictionary<string, List<Color>> colorAssociations;
        private Color DefaultColor;

        private string Username;

        private Random rng = new Random();

        public MainPage(bool letters, bool numbers, bool dow, bool months, string username)
        {
            InitializeComponent();

            Username = username;

            questions = new List<string>();
            bucket1 = new List<string>();
            bucket2 = new List<string>();
            bucket3 = new List<string>();
            if (letters)
            {
                List<string> q = Questions.Letters;
                AddQuestions(q);
            }
            if (numbers)
            {
                List<string> q = Questions.Numbers;
                AddQuestions(q);
            }
            if (dow)
            {
                List<string> q = Questions.DaysOfWeeks;
                AddQuestions(q);
            }
            if (months)
            {
                List<string> q = Questions.Months;
                AddQuestions(q);
            }

            Shuffle(bucket1);
            Shuffle(bucket2);
            Shuffle(bucket3);

            questions.AddRange(bucket1);
            questions.AddRange(bucket2);
            questions.AddRange(bucket3);

            colorAssociations = new Dictionary<string, List<Color>>();

            currentQuestionText = questions[questionCount];

            DefaultColor = Wheel.SelectedColor;

            BindingContext = this;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            NextButton.IsEnabled = false;

            Color color;
            if (Checkbox.IsChecked)
                color = Color.Transparent;
            else
                color = Wheel.SelectedColor;

            try
            {
                if (!colorAssociations.Keys.Contains<string>(currentQuestionText))
                {
                    colorAssociations.Add(currentQuestionText, new List<Color>());
                    
                }

                colorAssociations[currentQuestionText].Add(color);
            } catch (ArgumentException ex)
            {
                NextButton.IsEnabled = true;
                return;
            }
            

            if(questionCount == questions.Count - 1)
            {
                await Navigation.PushAsync(new ResultsPage(colorAssociations, Username));
                NextButton.IsEnabled = true;
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
            NextButton.IsEnabled = true;
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

        private void Shuffle(List<string> list)
        {
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void AddQuestions(List<string> q)
        {
            bucket1.AddRange(q);
            bucket2.AddRange(q);
            bucket3.AddRange(q);
        }
    }
}
