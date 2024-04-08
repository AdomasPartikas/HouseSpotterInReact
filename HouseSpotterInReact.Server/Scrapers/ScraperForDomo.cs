using HtmlAgilityPack;
using HouseSpotter.Server.Utils;
using System.Diagnostics;
using HouseSpotter.Server.Models;
using System.Text.RegularExpressions;
using HouseSpotter.Server.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace HouseSpotter.Server.Scrapers
{
    /// <summary>
    /// Represents a scraper for the Domo website.
    /// </summary>
    public class ScraperForDomo
    {
        private Random random = new Random();
        private ScraperClient _scraperClient;
        private HousingContext _housingContext;
        private ILogger<ScraperForDomo> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperForDomo"/> class.
        /// </summary>
        /// <param name="scraperClient">The scraper client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="housingContext">The housing context.</param>
        public ScraperForDomo(ScraperClient scraperClient, ILogger<ScraperForDomo> logger, HousingContext housingContext)
        {
            _scraperClient = scraperClient;
            _logger = logger;
            _housingContext = housingContext;
        }
        /// <summary>
        /// Finalizer that ensures the scrape is ended before the object is garbage collected.
        /// </summary>
        ~ScraperForDomo()
        {
            _scraperClient.EndScrape().Wait();
        }

        /// <summary>
        /// Ends the scrape process.
        /// </summary>
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
                ScrapedSite = ScrapedSite.Domo,
                ScrapeStatus = ScrapeStatus.Ongoing,
                DateScraped = DateTime.Now
            };

            for (int h = 0; h < 4; h++)
            {
                string siteEndpoint = h switch
                {
                    0 => "/namai-kotedzai-sodai?action_type=1",
                    1 => "/namai-kotedzai-sodai?action_type=3",
                    2 => "/butai?action_type=1",
                    3 => "/butai?action_type=3",
                    _ => "namai"
                };

                Debug.WriteLine($"[{DateTimeOffset.Now}] Running FindApartaments for Domo with {siteEndpoint} endpoint");

                string url = $"https://domoplius.lt/skelbimai{siteEndpoint}/";
                string pageUrl = url;
                int pageCount = 1;

                for (int i = 1; i <= 1; i++) // i<=should be pageCount
                {
                    Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                    if (i > 1)
                        pageUrl = url + $"&page_nr={i}";

                    string html = "";
                    var doc = new HtmlDocument();

                    try
                    {
                        if (!_scraperClient.NetworkPuppeteerClient.PuppeteerInitialized)
                        {
                            await _scraperClient.NetworkPuppeteerClient.Initialize();
                            await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://domoplius.lt/");
                            Thread.Sleep(125);
                        }

                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(pageUrl);
                        html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"[{DateTimeOffset.Now}]PuppeteerSharp failed to get {pageUrl}");
                        await EndScrape();
                        return new Scrape { Message = "PuppeteerSharp failed to get " + pageUrl, ScrapeStatus = ScrapeStatus.Failed, DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Domo, ScrapeType = ScrapeType.Full };
                    }

                    doc.LoadHtml(html);

                    if (pageCount == 1)
                    {
                        var pageSelect = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("cntnt-box-fixed")).FirstOrDefault();

                        var pageSelectCount = pageSelect!.Descendants("span")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("fl larger results-amount grey")).FirstOrDefault()!.InnerText;

                        string stringWithoutBrackets = pageSelectCount.Replace("(", "").Replace(")", "");

                        pageCount = (int)Math.Ceiling(Convert.ToDouble(stringWithoutBrackets)/30);

                    }

                    var abstractList = doc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("auto-list")).FirstOrDefault();
                    

                    var listOfSelections = abstractList!.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("dark-active not-viewed") || node.GetAttributeValue("class", "")
                    .Equals(" not-viewed")).ToList();

                    foreach (var item in listOfSelections)
                    {
                        var tip = "";
                        var aElement  = item.Descendants("a").FirstOrDefault();
                        if (aElement  != null)
                        {
                            tip = aElement.GetAttributeValue("href", "");
                        }

                        if (tip != null)
                        {

                            try
                            {
                                await SaveResult(true, tip, siteEndpoint);
                                _scraperClient.TotalQueries++;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {tip}");
                            }
                        }
                    }

                    Debug.WriteLine($"[{DateTimeOffset.Now}] <{i}/{pageCount}> Total queries made so far: {_scraperClient.TotalQueries} in /{siteEndpoint}/");
                }
            }
            _scraperClient.ScrapeEndDate = DateTime.Now;

            scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
            scrape.ScrapeStatus = ScrapeStatus.Success;
            scrape.TotalQueries = _scraperClient.TotalQueries;
            scrape.NewQueries = _scraperClient.NewQueries;
            scrape.Message = "Full scraping finished successfully.";

            await EndScrape();

            return scrape;
        }

        public async Task<Scrape> FindRecentHousingPosts()
        {
            _scraperClient.ScrapeStartDate = DateTime.Now;
            var scrape = new Scrape
            {
                ScrapeType = ScrapeType.Partial,
                ScrapedSite = ScrapedSite.Domo,
                ScrapeStatus = ScrapeStatus.Ongoing,
                DateScraped = DateTime.Now
            };

            for (int h = 0; h < 4; h++)
            {
                string siteEndpoint = h switch
                {
                    0 => "/namai-kotedzai-sodai?action_type=1",
                    1 => "/namai-kotedzai-sodai?action_type=3",
                    2 => "/butai?action_type=1",
                    3 => "/butai?action_type=3",
                    _ => "namai"
                };

                Debug.WriteLine($"[{DateTimeOffset.Now}] Running FindApartaments for Domo with {siteEndpoint} endpoint");

                string url = $"https://domoplius.lt/skelbimai{siteEndpoint}/";
                string pageUrl = url;
                int pageCount = 1;

                for (int i = 1; i <= 1; i++) // i<=should be pageCount
                {
                    Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                    if (i > 1)
                        pageUrl = url + $"&page_nr={i}";

                    string html = "";
                    var doc = new HtmlDocument();

                    try
                    {
                        if (!_scraperClient.NetworkPuppeteerClient.PuppeteerInitialized)
                        {
                            await _scraperClient.NetworkPuppeteerClient.Initialize();
                            await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://domoplius.lt/");
                            Thread.Sleep(125);
                        }

                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(pageUrl);
                        html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"[{DateTimeOffset.Now}]PuppeteerSharp failed to get {pageUrl}");
                        await EndScrape();
                        return new Scrape { Message = "PuppeteerSharp failed to get " + pageUrl, ScrapeStatus = ScrapeStatus.Failed, DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Domo, ScrapeType = ScrapeType.Full };
                    }

                    doc.LoadHtml(html);

                    if (pageCount == 1)
                    {
                        var pageSelect = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("cntnt-box-fixed")).FirstOrDefault();

                        var pageSelectCount = pageSelect!.Descendants("span")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("fl larger results-amount grey")).FirstOrDefault()!.InnerText;

                        string stringWithoutBrackets = pageSelectCount.Replace("(", "").Replace(")", "");

                        pageCount = (int)Math.Ceiling(Convert.ToDouble(stringWithoutBrackets)/30);

                    }

                    var abstractList = doc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("auto-list")).FirstOrDefault();
                    

                    var listOfSelections = abstractList!.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("dark-active not-viewed") || node.GetAttributeValue("class", "")
                    .Equals(" not-viewed")).ToList();

                    foreach (var item in listOfSelections)
                    {
                        var tip = "";
                        var aElement  = item.Descendants("a").FirstOrDefault();
                        if (aElement  != null)
                        {
                            tip = aElement .GetAttributeValue("href", "");
                        }

                        if (tip != null)
                        {
                            _scraperClient.TotalQueries++;

                            var existingHousing = await _housingContext.Housings.FirstOrDefaultAsync(h => h.Link == tip);

                            if (existingHousing == null)
                            {
                                try
                                {
                                    await SaveResult(true, tip, siteEndpoint);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {tip}");
                                }
                            }
                            else
                            {
                                _scraperClient.ScrapeEndDate = DateTime.Now;

                                scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
                                scrape.ScrapeStatus = ScrapeStatus.Success;
                                scrape.TotalQueries = _scraperClient.TotalQueries;
                                scrape.NewQueries = _scraperClient.NewQueries;
                                scrape.Message = "Partial scraping finished successfully.";

                                await EndScrape();

                                return scrape;
                            }
                        }
                    }

                    Debug.WriteLine($"[{DateTimeOffset.Now}] <{i}/{pageCount}> Total queries made so far: {_scraperClient.TotalQueries} in /{siteEndpoint}/");
                }
            }
            _scraperClient.ScrapeEndDate = DateTime.Now;

            scrape.ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate;
            scrape.ScrapeStatus = ScrapeStatus.Success;
            scrape.TotalQueries = _scraperClient.TotalQueries;
            scrape.NewQueries = _scraperClient.NewQueries;
            scrape.Message = "Full scraping finished successfully.";

            await EndScrape();

            return scrape;
        }

        private async Task SaveResult(bool savingDto, object result, string siteEndpoint = "")
        {
            if (savingDto)
            {
                if (result is Housing)
                {
                    var housing = result as Housing;

                    var existingHousing = await _housingContext.Housings.FirstOrDefaultAsync(h => h.Link == housing!.Link);

                    if (existingHousing != null)
                    {
                        existingHousing.AnketosKodas = housing!.AnketosKodas;
                        existingHousing.Nuotrauka = housing.Nuotrauka;
                        existingHousing.Link = housing.Link;
                        existingHousing.BustoTipas = housing.BustoTipas;
                        existingHousing.Title = housing.Title;
                        existingHousing.Kaina = housing.Kaina;
                        existingHousing.KainaMen = housing.KainaMen;
                        existingHousing.NamoNumeris = housing.NamoNumeris;
                        existingHousing.ButoNumeris = housing.ButoNumeris;
                        existingHousing.KambariuSk = housing.KambariuSk;
                        existingHousing.Plotas = housing.Plotas;
                        existingHousing.SklypoPlotas = housing.SklypoPlotas;
                        existingHousing.Aukstas = housing.Aukstas;
                        existingHousing.AukstuSk = housing.AukstuSk;
                        existingHousing.Metai = housing.Metai;
                        existingHousing.Irengimas = housing.Irengimas;
                        existingHousing.NamoTipas = housing.NamoTipas;
                        existingHousing.PastatoTipas = housing.PastatoTipas;
                        existingHousing.Sildymas = housing.Sildymas;
                        existingHousing.PastatoEnergijosSuvartojimoKlase = housing.PastatoEnergijosSuvartojimoKlase;
                        existingHousing.Ypatybes = housing.Ypatybes;
                        existingHousing.PapildomosPatalpos = housing.PapildomosPatalpos;
                        existingHousing.PapildomaIranga = housing.PapildomaIranga;
                        existingHousing.Apsauga = housing.Apsauga;
                        existingHousing.Vanduo = housing.Vanduo;
                        existingHousing.IkiTelkinio = housing.IkiTelkinio;
                        existingHousing.ArtimiausiasTelkinys = housing.ArtimiausiasTelkinys;
                        existingHousing.RCNumeris = housing.RCNumeris;
                        existingHousing.Aprasymas = housing.Aprasymas;

                        await _housingContext.SaveChangesAsync();
                    }
                    else
                    {
                        _housingContext.Housings.Add(housing!);
                        await _housingContext.SaveChangesAsync();
                    }
                }
                else if (result is string)
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
                                "/butai?action_type=3" => "butu-nuoma",
                                "/namai-kotedzai-sodai?action_type=3" => "namu-nuoma",
                                "/butai?action_type=1" => "butai",
                                "/namai-kotedzai-sodai?action_type=1" => "namai",
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

        /// <summary>
        /// Enriches new housings with details.
        /// All the new housing posts are enriched with details.
        /// </summary>
        /// <returns>The scrape result.</returns>
        public async Task<Scrape> EnrichNewHousingsWithDetails()
        {
            var housingList = await _housingContext.Housings.Select(h=>h).ToListAsync();

            _scraperClient.ScrapeStartDate = DateTime.Now;

            foreach (var housing in housingList)
            {
                try
                {
                    await GetHousingDetails(housing.Link, housing.BustoTipas);
                    _scraperClient.NewQueries++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{DateTimeOffset.Now}] Error while updating housing details for {housing.Link}");
                }
            }
            _scraperClient.TotalQueries = await _housingContext.Housings.CountAsync();
            _scraperClient.ScrapeEndDate = DateTime.Now;
            var result = new Scrape { Message = "Enriching housing details finished successfully.", DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Domo, ScrapeType = ScrapeType.Full, ScrapeStatus = ScrapeStatus.Success, TotalQueries = _scraperClient.TotalQueries, NewQueries = _scraperClient.NewQueries, ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate };

            await EndScrape();

            return result;
        }


        private async Task GetHousingDetails(string url, string bustoTipas)
        {
            if(url.StartsWith("https://domoplius.lt/"))
                {
                Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                string html = "";
                var doc = new HtmlDocument();

                var house = new Housing();

                try
                {
                    if (!_scraperClient.NetworkPuppeteerClient.PuppeteerInitialized)
                    {
                        await _scraperClient.NetworkPuppeteerClient.Initialize();
                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://domoplius.lt/");
                        Thread.Sleep(125);
                    }

                    await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(url);
                    html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"[{DateTimeOffset.Now}] PuppeteerSharp failed to get {url}");
                    return;
                }

                doc.LoadHtml(html);

                var header = doc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                        .Equals("medium info-block")).ToList();


                double price = 0;

                var titleElement = doc.DocumentNode.Descendants("h1")
                                  .FirstOrDefault(node => node.GetAttributeValue("class", "")
                                  .Equals("fl title-view"));

                string title = titleElement != null ? titleElement.InnerText.Trim() : "N/A";

                if (header.Count != 0)
                {
                    try
                    {
                        var priceElement = doc.DocumentNode.Descendants("strong")
                                            .Where(node => node.GetAttributeValue("class", "") == "h1 fl")
                                            .FirstOrDefault();
                        if (priceElement != null)
                        {
                            string priceText = priceElement.InnerText.Trim();
                            priceText = priceText.Replace(" ", "").Replace("€", "");
                            price = Convert.ToDouble(priceText);
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Error: getting price of {url}");
                    }
                }

                house.Link = url;
                house.Title = title;
                house.Kaina = price;
                house.BustoTipas = bustoTipas;
                house.AnketosKodas = Regex.Match(url, @"\d+(?=\D*$)").Value;
                
                foreach (var block in header)
                {
                    // Find the div element within the 'medium info-block' div
                    var divElement = block.Descendants("div").FirstOrDefault(div => string.IsNullOrWhiteSpace(div.GetAttributeValue("class", "")));

                    // Check if the div element is not null and contains some text
                    if (divElement != null && !string.IsNullOrWhiteSpace(divElement.InnerText.Trim()))
                    {
                        // Assign the inner text of the div to house.Aprasymas
                        house.Aprasymas = divElement.InnerText.Trim();
                        
                        // Exit the loop since we've assigned the description
                        break;
                    }
                }

            var extractedDataList = new List<(string Title, string Definition)>();

            var abst = doc.DocumentNode.Descendants("table")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("view-group") || node.GetAttributeValue("class", "")
                .Equals("view-group price-format")).ToList();

            foreach (var table in abst)
            {
                var descriptionList = table.Descendants("tr").ToList();
                
                // Assuming each table has the same structure, you can loop through descriptionList to extract data
                foreach (var row in descriptionList)
                {
                    var descriptionTitle = row.Descendants("th").FirstOrDefault()?.InnerText.Trim();
                    var descriptionDefinition = row.Descendants("td").FirstOrDefault()?.InnerText.Trim();


                    // Add extracted data to the list
                    extractedDataList.Add((descriptionTitle, descriptionDefinition));
                }
            }

            foreach (var data in extractedDataList)
            {
                var dscTitle = data.Title;
                var dscDefinition = data.Definition;

                if (data.Title.Contains("Nuomos kaina"))
                {
                    house.KainaMen = Convert.ToInt32(dscDefinition.Split('€')[0].Trim().Replace(" ", ""));
                }

                switch (dscTitle)
                {
                    case "Namo numeris:":
                        {
                            house.NamoNumeris = dscDefinition;
                        }
                        break;
                    case "Kambarių skaičius:":
                        {
                            house.KambariuSk = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Bendras pastato plotas:":
                    case "Buto plotas (kv. m):":
                    {
                        dscDefinition = dscDefinition.Replace(" kv. m.", "").Trim();
                        house.Plotas = Convert.ToDouble(dscDefinition);
                    }
                    break;
                    case "Aukštas":
                        {
                            house.Aukstas = Convert.ToInt32(dscDefinition.First().ToString());
                            house.AukstuSk = Convert.ToInt32(dscDefinition.Reverse().FirstOrDefault(char.IsDigit).ToString());
                        }
                        break;
                    case "Aukštų skaičius:":
                        {
                            house.AukstuSk = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    
                    case "Statybos metai:":
                        {
                            house.Metai = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Būklė":
                    case "Namo būklė":
                        {
                            house.Irengimas = dscDefinition;
                        }
                        break;
                    case "Šildymas:":
                        {
                            house.Sildymas = dscDefinition;
                        }
                        break;
                    case "Energinio naudingumo klasė:":
                        {
                            house.PastatoEnergijosSuvartojimoKlase = dscDefinition;
                        }
                        break;
                    case "Vandentiekis:":
                        {
                            house.Vanduo = dscDefinition;
                        }
                        break;
                    case "Unikalus daikto numeris (RC numeris)":
                        {
                            house.RCNumeris = dscDefinition;
                        }
                        break;
                    case "Namo tipas:":
                        {
                            house.PastatoTipas = dscDefinition;
                        }
                        break;
                    case "Sklypo plotas:":
                        {
                            house.SklypoPlotas = dscDefinition;
                        }
                        break;
                    default:
                        {
                            Debug.WriteLine($"Title not defined: {dscTitle}");
                            Thread.Sleep(1);
                        }
                        break;
                }
            }
                

                await SaveResult(true, house);
                return;
            }
        }


    }
}