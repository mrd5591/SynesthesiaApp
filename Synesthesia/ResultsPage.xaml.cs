using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Synesthesia.Models;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using CsvHelper;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Synesthesia
{
    public partial class ResultsPage : ContentPage
    {
        private Dictionary<string, List<Color>> colorMapping;
        private string tempFile;
        private string Username;
        public ResultsPage(Dictionary<string, List<Color>> colorMapping, string username)
        {
            InitializeComponent();

            this.Username = username;

            this.colorMapping = colorMapping;
        }

        public async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string selection = Picker.Items[Picker.SelectedIndex];

            if (selection.Equals("CSV"))
            {
                CreateCSV();
            }
            else
            {
                CreateXLSX();
            }

            string email = EmailEntry.Text;

            await SendEmail(email);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

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
            tempFile = Path.Combine(Path.GetTempPath(), "synesthesiaData.csv");
            var records = new List<CsvColor>();

            foreach(KeyValuePair<string, List<Color>> pair in colorMapping)
            {
                List<Color> colors = pair.Value;
                foreach(Color c in colors)
                    records.Add(new CsvColor {Stimulus = pair.Key, Red = c.R, Green = c.G, Blue = c.B, Alpha = c.A, Hue = c.Hue, Luminosity = c.Luminosity });
            }

            using (var writer = new StreamWriter(tempFile))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        private void CreateXLSX()
        {
            tempFile = Path.Combine(Path.GetTempPath(), "synesthesiaData.xlsx");
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet 1");

            IRow row0 = sheet.CreateRow(0);
            XSSFCellStyle trialStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            trialStyle.Alignment = HorizontalAlignment.Center;
            for (int j = 0; j < 19; j++)
            {
                ICell cell = row0.CreateCell(j);
                cell.CellStyle = trialStyle;
            }

            row0.GetCell(1).SetCellValue("Trial 1");
            row0.GetCell(8).SetCellValue("Trial 2");
            row0.GetCell(15).SetCellValue("Trial 3");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 1, 7));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 8, 14));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 15, 21));

            IRow row1 = sheet.CreateRow(1);
            row1.CreateCell(0).SetCellValue("Stimulus");
            for (int j = 0; j < 3; j++)
            {
                row1.CreateCell(j * 7 + 1).SetCellValue("Red");
                row1.CreateCell(j * 7 + 2).SetCellValue("Green");
                row1.CreateCell(j * 7 + 3).SetCellValue("Blue");
                row1.CreateCell(j * 7 + 4).SetCellValue("Alpha");
                row1.CreateCell(j * 7 + 5).SetCellValue("Hue");
                row1.CreateCell(j * 7 + 6).SetCellValue("Luminosity");
                row1.CreateCell(j * 7 + 7).SetCellValue("Color");
            }

            int i = 2;
            foreach (KeyValuePair<string, List<Color>> pair in colorMapping)
            {
                int col = 0;
                IRow row = sheet.CreateRow(i);
                List<Color> colors = pair.Value;

                row.CreateCell(col).SetCellValue(pair.Key);

                foreach(Color c in colors)
                {
                    row.CreateCell(++col).SetCellValue(c.R);
                    row.CreateCell(++col).SetCellValue(c.G);
                    row.CreateCell(++col).SetCellValue(c.B);
                    row.CreateCell(++col).SetCellValue(c.A);
                    row.CreateCell(++col).SetCellValue(c.Hue);
                    row.CreateCell(++col).SetCellValue(c.Luminosity);

                    ICell colorCell = row.CreateCell(++col);
                    XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();
                    style.FillForegroundXSSFColor = new XSSFColor(c);
                    style.FillPattern = FillPattern.SolidForeground;
                    colorCell.CellStyle = style;
                }

                i++;
            }

            FileStream sw = File.Create(tempFile);
            workbook.Write(sw);
            sw.Close();
        }

        public async Task SendEmail(string email)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Your Synesthesia Data!",
                    Body = "The requested file type is attached. This email is from user: " + Username,
                    To = new List<string> { email },
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
