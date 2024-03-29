﻿using System;
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
        private List<StimulusGroup> StimulusGroups;
        private string tempFile;
        private string Username;
        public double overallScore { get; set; }

        public double TextFontSize { get; set; }
        public double HeaderFontSize { get; set; }

        public ResultsPage(List<StimulusGroup> stimulusGroups/*, string username*/, double score)
        {
            InitializeComponent();

            //this.Username = username;

            this.StimulusGroups = stimulusGroups;

            this.overallScore = Math.Round(score, 2);

            TextFontSize = App.TextFontSize;
            HeaderFontSize = App.HeaderFontSize;

            //UID.Text = username;
            Score.Text = this.overallScore.ToString();

            BindingContext = this;
        }

        async void RestartTestButton_Clicked(System.Object sender, System.EventArgs e)
        {
            for (var counter = 1; counter < 3; counter++)
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }
            await Navigation.PopAsync();
        }

        void CloseButton_Clicked(System.Object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /*public async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            CreateXLSX();
            //string email = EmailEntry.Text;

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
            if (!string.IsNullOrWhiteSpace(email) && IsValidEmail(email))
            {
                ResultButton.IsEnabled = true;
            } else
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

            for(int j = 0; j < 22; j++)
            {
                row0.CreateCell(j);
            }

            XSSFCellStyle trialText = (XSSFCellStyle)workbook.CreateCellStyle();
            trialText.Alignment = HorizontalAlignment.Center;
            trialText.BorderLeft = BorderStyle.Thick;
            trialText.BorderRight = BorderStyle.Thick;

            XSSFCellStyle leftBoldBorder = (XSSFCellStyle)workbook.CreateCellStyle();
            leftBoldBorder.BorderLeft = BorderStyle.Thick;

            XSSFCellStyle rightBoldBorder = (XSSFCellStyle)workbook.CreateCellStyle();
            rightBoldBorder.BorderRight = BorderStyle.Thick;

            row0.GetCell(1).SetCellValue("Trial 1");
            row0.GetCell(2).SetCellValue("Trial 2");
            row0.GetCell(3).SetCellValue("Trial 3");

            row0.GetCell(4).SetCellValue("Trial 1");
            row0.GetCell(4).CellStyle = trialText;
            row0.GetCell(11).SetCellValue("Trial 2");
            row0.GetCell(11).CellStyle = trialText;
            row0.GetCell(18).SetCellValue("Trial 3");
            row0.GetCell(18).CellStyle = trialText;
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 4, 9));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 10, 15));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 16, 21));

            IRow row1 = sheet.CreateRow(1);
            row1.CreateCell(0).SetCellValue("Stimulus");
            row1.CreateCell(1).SetCellValue("Color");
            row1.CreateCell(2).SetCellValue("Color");
            row1.CreateCell(3).SetCellValue("Color");
            for (int j = 0; j < 3; j++)
            {
                row1.CreateCell(j * 6 + 4).SetCellValue("Red");
                row1.GetCell(j * 6 + 4).CellStyle = leftBoldBorder;
                row1.CreateCell(j * 6 + 5).SetCellValue("Green");
                row1.CreateCell(j * 6 + 6).SetCellValue("Blue");
                row1.CreateCell(j * 6 + 7).SetCellValue("Alpha");
                row1.CreateCell(j * 6 + 8).SetCellValue("Hue");
                row1.CreateCell(j * 6 + 9).SetCellValue("Luminosity");
                row1.GetCell(j * 6 + 9).CellStyle = rightBoldBorder;
            }

            int i = 2;
            foreach (StimulusGroup group in StimulusGroups)
            {
                foreach(StimulusResult res in group.Stimuli)
                {
                    int col = 0;
                    IRow row = sheet.CreateRow(i);
                    List<Color> colors = res.Colors;

                    row.CreateCell(col).SetCellValue(res.Name);

                    int colIndex = 0;
                    foreach (Color c in colors)
                    {
                        ICell colorCell = row.CreateCell(++col);
                        if (c.Equals(Color.Transparent))
                        {
                            row.GetCell(col).SetCellValue("None");
                        }
                        else
                        {
                            row.GetCell(col).SetCellValue("\t");
                            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();
                            style.FillForegroundXSSFColor = new XSSFColor(c);
                            style.FillPattern = FillPattern.SolidForeground;
                            colorCell.CellStyle = style;
                        }

                        int t = 3 + (col - 1) * 6 + ++colIndex;
                        row.CreateCell(t).SetCellValue(c.R);
                        row.GetCell(t).CellStyle = leftBoldBorder;
                        row.CreateCell(3 + (col - 1) * 6 + ++colIndex).SetCellValue(c.G);
                        row.CreateCell(3 + (col - 1) * 6 + ++colIndex).SetCellValue(c.B);
                        row.CreateCell(3 + (col - 1) * 6 + ++colIndex).SetCellValue(c.A);
                        row.CreateCell(3 + (col - 1) * 6 + ++colIndex).SetCellValue(c.Hue);
                        t = 3 + (col - 1) * 6 + ++colIndex;
                        row.CreateCell(t).SetCellValue(c.Luminosity);
                        row.GetCell(t).CellStyle = rightBoldBorder;

                        colIndex = 0;
                    }

                    i++;
                }

                IRow scoreRow = sheet.CreateRow(++i);
                scoreRow.CreateCell(0).SetCellValue("Group Score");
                scoreRow.CreateCell(2).SetCellValue(group.Score);
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 1));

                IRow textRow = sheet.CreateRow(++i);
                ICell textCell = textRow.CreateCell(0);
                textCell.SetCellValue("Here are the colors you chose for each trial. Lower scores (below 1.0) indicate that you are very good at making consistent associations.");
                XSSFCellStyle style2 = (XSSFCellStyle)workbook.CreateCellStyle();
                style2.Alignment = HorizontalAlignment.Center;
                style2.WrapText = true;
                style2.VerticalAlignment = VerticalAlignment.Top;
                textCell.CellStyle = style2;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i + 3, 0, 3));

                i += 6;
            }

            IRow uidRow = sheet.CreateRow(i);
            uidRow.CreateCell(0).SetCellValue("UID");
            uidRow.CreateCell(1).SetCellValue(Username);
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 1, 2));

            IRow overallScoreRow = sheet.CreateRow(++i);
            overallScoreRow.CreateCell(0).SetCellValue("Overall Score");
            overallScoreRow.CreateCell(2).SetCellValue(overallScore);
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, 1));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 2, 3));
            
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
        }*/
    }
}
