using System;
using System.Linq;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using CsvHelper;
using System.Formats.Asn1;
using CsvHelper.Configuration;
using System.Globalization;

/* changes made:
 * - Target classes changed
 * - Link changed (change it yourself for your location lol)
 * - config specified
 * - records list constructor made
 * - write records using list constructor
 */

namespace WeatherScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Download the HTML from weather.com -- change to your location appropriately
            var html = new WebClient().DownloadString("https://weather.com/weather/tenday/l/Cleveland+OH");

            // Load the HTML into an HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Get all the forecast elements
            var forecastElements = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'DaypartDetails--DetailSummaryContent--1-r0i')]");

            // config
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

            // Create a CSV file
            using (var csvfile = File.CreateText("10-day-forecast.csv"))
            {
                var csvWriter = new CsvWriter(csvfile, config);

                // For each forecast element, write the date and forecast to the CSV file
                foreach (var element in forecastElements)
                {
                    var date = element.SelectSingleNode(".//h3[contains(@class, 'DetailsSummary--daypartName--kbngc')]").InnerText.Trim();
                    var forecast = element.SelectSingleNode(".//div[contains(@class, 'DetailsSummary--DetailsSummary--1DqhO')]").InnerText.Trim();
                    var wxrecords = new List<records>
                    {
                        new records {date=date,forecast=forecast},
                    };
                    csvWriter.WriteRecords(wxrecords);
                }
            }
        }
        public class records
        {
            public string date { get; set; }
            public string forecast { get; set; }
        }
    }
}
