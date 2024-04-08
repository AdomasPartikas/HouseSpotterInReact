using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using HouseSpotter.Server.Context;
using HouseSpotter.Server.Models;
using HouseSpotter.Server.Scrapers;
using HouseSpotter.Server.Utils;
using HtmlAgilityPack;

namespace HouseSpotter.Server.Scrapers
{
    public class ScraperForSkelbiu
    {
        private Random random = new Random();
        private ScraperClient _scraperClient;
        private HousingContext _housingContext;
        private ILogger<ScraperForSkelbiu> _logger;
        public ScraperForSkelbiu(ScraperClient scraperClient, ILogger<ScraperForSkelbiu> logger, HousingContext housingContext)
        {
            _scraperClient = scraperClient;
            _logger = logger;
            _housingContext = housingContext;
        }
        ~ScraperForSkelbiu()
        {
            _scraperClient.EndScrape().Wait();
        }
        private async Task EndScrape()
        {
            await _scraperClient.EndScrape();
        }
        public async Task<Scrape> FindAllHousingPosts()
        {
            _scraperClient.ScrapeStartDate = DateTime.Now;
            var scrape = new Scrape
            {
                ScrapeType = ScrapeType.Full,
                ScrapedSite = ScrapedSite.Skelbiu,
                ScrapeStatus = ScrapeStatus.Ongoing,
                DateScraped = DateTime.Now
            };

            for(int i = 0; i < 2; i++)
            {
                string siteEndpoint = i switch
                {
                    0 => "namai",
                    1 => "butai",
                    _ => throw new Exception("Invalid endpoint")
                };

                Debug.WriteLine($"[{DateTimeOffset.Now}] Running FindApartaments for Skelbiu with {siteEndpoint} endpoint");

                string url = $"https://m.skelbiu.lt/skelbimai/nekilnojamasis-turtas/{siteEndpoint}/";
                string pageUrl = url;
                int pageCount = 1;

                for(int j = 1; j <= 1; j++) //change 1 to pageCount
                {
                    Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                    if (i > 1)
                        pageUrl = url + $"{i}/";

                    string html = "";
                    var doc = new HtmlDocument();

                    try
                    {
                        if (!_scraperClient.NetworkPuppeteerClient.PuppeteerInitialized)
                        {
                            await _scraperClient.NetworkPuppeteerClient.Initialize();
                            await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://m.skelbiu.lt/");
                            Thread.Sleep(125);
                        }

                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(pageUrl);
                        html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"[{DateTimeOffset.Now}]PuppeteerSharp failed to get {pageUrl}");
                        await EndScrape();

                        scrape.ScrapeStatus = ScrapeStatus.Failed;
                        scrape.Message = "PuppeteerSharp failed to get " + pageUrl;
                        scrape.ScrapeTime = DateTime.Now - _scraperClient.ScrapeStartDate;

                        return scrape;
                    }

                    doc.LoadHtml(html);

                    if(pageCount == 1)
                    {
                        var pageSelect = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("id", "")
                            .Equals("pagingArea")).FirstOrDefault();

                        var pagingLink = doc.DocumentNode.Descendants("a")
                            .Where(node => node.GetAttributeValue("id", "")
                            .Equals("pagingLink")).FirstOrDefault()!.InnerText;

                        var pattern = @"1 - (\d+) iš (\d+)";
                        var match = Regex.Match(pagingLink, pattern);

                        if (match.Success)
                        {
                            double value = double.Parse(match.Groups[2].Value) / double.Parse(match.Groups[1].Value);
                            pageCount = (int)Math.Ceiling(value);
                        }
                    }

                    
                }
            }

                _scraperClient.ScrapeEndDate = DateTime.Now;

                scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
                scrape.ScrapeStatus = ScrapeStatus.Success;
                scrape.TotalQueries = _scraperClient.TotalQueries;
                scrape.NewQueries = _scraperClient.NewQueries;

                await EndScrape();

                return scrape;
        }
    }
}