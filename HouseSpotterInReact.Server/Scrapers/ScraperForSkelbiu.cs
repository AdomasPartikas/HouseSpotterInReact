using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using HouseSpotter.Server.Context;
using HouseSpotter.Server.Models;
using HouseSpotter.Server.Scrapers;
using HouseSpotter.Server.Utils;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Enriches new housings with details.
        /// All the new housing posts are enriched with details.
        /// </summary>
        /// <returns>The scrape result.</returns>
        public async Task<Scrape> EnrichNewHousingsWithDetails()
        {
            var housingList = await _housingContext.Housings.Where(h => !String.IsNullOrEmpty(h.Link) 
            && String.IsNullOrEmpty(h.AnketosKodas) 
            && h.Link.StartsWith("https://m.skelbiu.lt/")).ToListAsync();

            _scraperClient.ScrapeStartDate = DateTime.Now;
            var scrape = new Scrape
            {
                ScrapeType = ScrapeType.Full,
                ScrapedSite = ScrapedSite.Skelbiu,
                ScrapeStatus = ScrapeStatus.Ongoing,
                DateScraped = DateTime.Now
            };

            foreach (var housing in housingList)
            {
                try
                {
                    await GetHousingDetails(housing);
                    _scraperClient.NewQueries++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{DateTimeOffset.Now}] Error while updating housing details for {housing.Link}");
                }
            }

            _scraperClient.ScrapeEndDate = DateTime.Now;

            scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
            scrape.ScrapeStatus = ScrapeStatus.Success;
            scrape.TotalQueries = await _housingContext.Housings.CountAsync();
            scrape.NewQueries = _scraperClient.NewQueries;
            scrape.Message = "Enriching housing details finished successfully.";

            await EndScrape();

            return scrape;
        }

        private async Task GetHousingDetails(Housing house)
        {
            Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

            string html = "";
            var doc = new HtmlDocument();

            try
            {
                if (!_scraperClient.NetworkPuppeteerClient.PuppeteerInitialized)
                {
                    await _scraperClient.NetworkPuppeteerClient.Initialize();
                    await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://m.aruodas.lt/");
                    Thread.Sleep(125);
                }

                await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(house.Link);
                html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"[{DateTimeOffset.Now}] PuppeteerSharp failed to get {house.Link}");
                return;
            }

            doc.LoadHtml(html);

            var header = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("bd")).FirstOrDefault();

            var imageHolder = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("photoArea")).FirstOrDefault();

            var img = imageHolder!.Descendants("img").FirstOrDefault()!.GetAttributeValue("src", "");

            house.Title = header!.Descendants("h1")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("item-title")).FirstOrDefault()!.InnerText;

            house.Kaina = Convert.ToDouble(header!.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("price")).FirstOrDefault()!.FirstChild.InnerText.Replace(" ", "").Trim().TrimEnd('€'));

            house.AnketosKodas = Regex.Match(house.Link!, @"(\d+)\.html").Groups[1].Value;

            house.Aprasymas = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("description")).FirstOrDefault()!.InnerText;

            var detailTextList = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("detailsMoreArea"))
                .FirstOrDefault()!.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("dataText")).ToList();

            var detailInfoList = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("detailsMoreArea"))
                .FirstOrDefault()!.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("dataInfo")).ToList();

            var features = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("detailsMoreArea"))
                .FirstOrDefault()!.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("dataDetailsArea")).FirstOrDefault()!.InnerText;

            for (int i = 0; i < detailTextList.Count; i++)
            {
                var text = detailTextList[i].InnerText.Trim();
                var info = detailInfoList[i].InnerText.Trim();

                switch(text)
                {
                    case "Gyvenvietė:":
                    {
                        house.Gyvenviete = info;
                    }break;
                    case "Gatve:":
                    {
                        house.Gatve = info;
                    }break;
                    case "Įrengimas:":
                    {
                        house.Irengimas = info;
                    }break;
                    case "Pastato tipas:":
                    {
                        house.NamoTipas = info;
                    }break;
                    case "Tipas:":
                    {
                        house.PastatoTipas = info;
                    }break;
                    case "Metai:":
                    {
                        house.Metai = Convert.ToInt32(Regex.Match(info, @"(\d{4})").Groups[1].Value);
                    }break;
                    case "Plotas, m²:":
                    {
                        house.Plotas = Convert.ToDouble(info.Replace('²', ' ').Replace('m', ' ').Trim());
                    }break;
                    case "Aukštas:":
                    {
                        house.Aukstas = Convert.ToInt32(info);
                    }break;
                    case "Aukštų skaičius:":
                    {
                        house.AukstuSk = Convert.ToInt32(info);
                    }break;
                    case "Kamb. sk.:":
                    {
                        house.KambariuSk = Convert.ToInt32(info);
                    }break;
                    case "Sklypo plotas, a:":
                    {
                        house.SklypoPlotas = info;
                    }break;
                    case "Šildymas:":
                    {
                        house.Sildymas = info;
                    }break;
                    case "Namo numeris:":
                    {
                        house.NamoNumeris = info;
                    }break;
                    case "Energetinė klasė:":
                    {
                        house.PastatoEnergijosSuvartojimoKlase = info;
                    }break;
                    default:
                    {
                        _logger.LogWarning($"[{DateTimeOffset.Now}] Unhandled detail: {text}");
                    }break;
                }
            }

            if(features != null)
            {
                var featuresList = features.Split(", ");
                house.Ypatybes = string.Join("/", featuresList);
            }
        }

        public async Task<Scrape> FindRecentHousingPosts()
        {
            _scraperClient.ScrapeStartDate = DateTime.Now;
            var scrape = new Scrape
            {
                ScrapeType = ScrapeType.Partial,
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
                        pageUrl = url + $"{i}?orderBy=1";

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

                    var housingPosts = doc.DocumentNode
                        .Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("gallery-list")).FirstOrDefault()!
                        .Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("item")).ToList();

                    foreach (var post in housingPosts)
                    {
                        var tip = post.Descendants("a")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("gallery-item-element-link js-cfuser-link"))
                            .FirstOrDefault();

                        if (tip != null)
                        {
                            var text = "https://m.skelbiu.lt" + tip.GetAttributeValue("href", "");

                            _scraperClient.TotalQueries++;

                            var existingHousing = await _housingContext.Housings.FirstOrDefaultAsync(h => h.Link == text);

                            if (existingHousing == null)
                            {
                                try
                                {
                                    await SaveResult(true, text, siteEndpoint);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {text}");
                                }
                            }
                            else
                            {
                                _scraperClient.ScrapeEndDate = DateTime.Now;

                                scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
                                scrape.ScrapeStatus = ScrapeStatus.Success;
                                scrape.TotalQueries = _scraperClient.TotalQueries;
                                scrape.NewQueries = _scraperClient.NewQueries;
                                scrape.Message = "Scrape finished successfully";

                                await EndScrape();

                                return scrape;
                            }
                        }
                    }
                    
                }
            }

                _scraperClient.ScrapeEndDate = DateTime.Now;

                scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
                scrape.ScrapeStatus = ScrapeStatus.Success;
                scrape.TotalQueries = _scraperClient.TotalQueries;
                scrape.NewQueries = _scraperClient.NewQueries;
                scrape.Message = "Scrape finished successfully";

                await EndScrape();

                return scrape;
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
                        pageUrl = url + $"{i}";

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

                    var housingPosts = doc.DocumentNode
                        .Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("gallery-list")).FirstOrDefault()!
                        .Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("item")).ToList();

                    foreach (var post in housingPosts)
                    {
                        var tip = post.Descendants("a")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("gallery-item-element-link js-cfuser-link"))
                            .FirstOrDefault();

                        if (tip != null)
                        {
                            var text = "https://m.skelbiu.lt" + tip.GetAttributeValue("href", "");

                            try
                            {
                                await SaveResult(true, text, siteEndpoint);
                                _scraperClient.TotalQueries++;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {text}");
                            }
                        }
                    }
                    
                }
            }

                _scraperClient.ScrapeEndDate = DateTime.Now;

                scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
                scrape.ScrapeStatus = ScrapeStatus.Success;
                scrape.TotalQueries = _scraperClient.TotalQueries;
                scrape.NewQueries = _scraperClient.NewQueries;
                scrape.Message = "Scrape finished successfully";

                await EndScrape();

                return scrape;
        }

        private async Task SaveResult(bool sendingToDatabase, object result, string siteEndpoint)
        {
            if (sendingToDatabase)
            {
                if (result is Housing) //Means it's a dto
                {
                    var housing = result as Housing;
                    
                    _housingContext.Housings.Add(housing!);
                    await _housingContext.SaveChangesAsync();
                }
                else if (result is string) //Means it's a link
                {
                    var text = result as String;

                    var existingHousing = await _housingContext.Housings.FirstOrDefaultAsync(h => h.Link == text);

                    if (existingHousing == null)
                    {
                        _scraperClient.NewQueries++;

                        var housing = new Housing
                        {
                            Link = text,
                            BustoTipas = siteEndpoint switch
                            {
                                "butai" => "butai",
                                "namai" => "namai",
                                _ => null
                            }
                        };

                        _housingContext.Housings.Add(housing);
                        await _housingContext.SaveChangesAsync();
                    }
                }
                else
                {
                    _logger.LogError($"[{DateTimeOffset.Now}] Error while saving result");
                }
            }
            else
            {
                var text = result as String;
                if (_scraperClient.UsingSql)
                {
                    //EstablishSqlConnection() turbut
                    //Something something something.....
                    //DestroySqlConnection() turbut
                }
                else
                {
                    //Debug.WriteLine($"[{DateTimeOffset.Now}] {text}");
                }
            }
        }
    }
}