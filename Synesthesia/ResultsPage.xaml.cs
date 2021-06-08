using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Synesthesia.Models;
using Rg.Plugins.Popup.Services;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.IO;
using System.Text;
using CsvHelper;
using System.Threading.Tasks;
using Xamarin.Essentials;

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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string selection = Picker.Items[Picker.SelectedIndex];

            string fileType;

            if (selection.Equals("CSV"))
            {
                //fileType = "csv";
                CreateCSV();
            }
            else
            {
                //fileType = "xlsx";
                CreateXLSX();
            }

            string email = EmailEntry.Text;

            await SendEmail(email);

            /*SubmitJsonPackage pkg = new SubmitJsonPackage();
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
            await PopupNavigation.Instance.PopAsync();

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

        private void CreateCSV()
        {
            tempFile = Path.GetTempFileName() + ".csv";
            var records = new List<string[]>
            {
                new string[] {"Stimulus", "Red", "Green", "Blue", "Alpha", "Hue", "Luminosity"}
            };

            foreach(KeyValuePair<string, Color> pair in colorMapping)
            {
                Color c = pair.Value;
                records.Add(new string[] {pair.Key, c.R.ToString(), c.G.ToString(), c.B.ToString(), c.A.ToString(), c.Hue.ToString(), c.Luminosity.ToString() });
            }

            using (var writer = new StreamWriter(tempFile))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        private void CreateXLSX()
        {
            tempFile = Path.GetTempFileName() + ".xlsx";
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Title = "SynesthesiaResults";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "Stimuls";
                worksheet.Cells["A2"].Value = "Red";
                worksheet.Cells["A3"].Value = "Green";
                worksheet.Cells["A4"].Value = "Blue";
                worksheet.Cells["A5"].Value = "Alpha";
                worksheet.Cells["A6"].Value = "Hue";
                worksheet.Cells["A7"].Value = "Luminosity";

                int row = 2;
                foreach (KeyValuePair<string, Color> pair in colorMapping)
                {
                    int col = 1;
                    Color c = pair.Value;
                    worksheet.Cells[col, row].Value = pair.Key;
                    worksheet.Cells[++col, row].Value = c.R;
                    worksheet.Cells[++col, row].Value = c.G;
                    worksheet.Cells[++col, row].Value = c.B;
                    worksheet.Cells[++col, row].Value = c.A;
                    worksheet.Cells[++col, row].Value = c.Hue;
                    worksheet.Cells[++col, row].Value = c.Luminosity;

                    row++;
                }

                FileInfo fi = new FileInfo(tempFile);
                excelPackage.SaveAs(fi);
            }
        }

        public async Task SendEmail(string email)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Your Synesthesia Data!",
                    Body = "The requested file type is attached.",
                    To = new List<string> {email },
                };

                message.Attachments.Add(new EmailAttachment(tempFile));

                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                Console.WriteLine(fbsEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
