using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Synesthesia.Models;
using Rg.Plugins.Popup.Services;
using Newtonsoft.Json.Linq;

namespace Synesthesia
{
    public partial class ResultsPage : ContentPage
    {
        private Dictionary<String, Color> colorMapping;
        private string tempFile;
        public ResultsPage(Dictionary<String, Color> colorMapping)
        {
            InitializeComponent();

            this.colorMapping = colorMapping;
        }

        public async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string selection = Picker.Items[Picker.SelectedIndex];

            string fileType;

            if(selection.Equals("CSV"))
            {
                fileType = "csv";
            } else
            {
                fileType = "xlsx";
            }

            string email = EmailEntry.Text;

            SubmitJsonPackage pkg = new SubmitJsonPackage();
            pkg.email = email;
            pkg.fileType = fileType;

            Dictionary<string, ColorJson> colors = new Dictionary<string, ColorJson>();
            foreach (KeyValuePair<string, Color> entry in colorMapping)
            {
                Color c = entry.Value;
                ColorJson json = new ColorJson();
                json.r = c.R;
                json.g = c.G;
                json.b = c.B;
                json.a = c.A;
                json.h = c.Hue;
                json.l = c.Luminosity;
                colors.Add(entry.Key, json);
            }

            pkg.colors = colors;

            await PopupNavigation.Instance.PushAsync(new PopupPage());
            String result = RESTClient.Post(new Uri(App.url + "submit"), JsonConvert.SerializeObject(pkg));
            /*await PopupNavigation.Instance.PopAsync();

            JObject rss = JObject.Parse(result);

            bool successful = (bool)rss["successfullySent"];

            if (successful)
                await Application.Current.MainPage.DisplayAlert("Success", "The data was successfully sent to your email.", "OK");
            else
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred. Please try again.", "OK");*/
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        void Entry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            string email = ((Entry)sender).Text;
            if (!string.IsNullOrWhiteSpace(email) && IsValidEmail(email) && Picker.SelectedItem != null)
            {
                ResultButton.IsEnabled = true;
            } else
            {
                ResultButton.IsEnabled = false;
            }
        }

        void Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            string email = EmailEntry.Text;
            if (!string.IsNullOrWhiteSpace(email) && IsValidEmail(email) && Picker.SelectedItem != null)
            {
                ResultButton.IsEnabled = true;
            }
            else
            {
                ResultButton.IsEnabled = false;
            }
        }
    }
}
