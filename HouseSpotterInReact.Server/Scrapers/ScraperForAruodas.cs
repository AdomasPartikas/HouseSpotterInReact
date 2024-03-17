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
    public class ScraperForAruodas
    {
        private Random random = new Random();
        private ScraperClient _scraperClient;
        private HousingContext _housingContext;
        private ILogger<ScraperForAruodas> _logger;
        public ScraperForAruodas(ScraperClient scraperClient, ILogger<ScraperForAruodas> logger, HousingContext housingContext)
        {
            _scraperClient = scraperClient;
            _logger = logger;
            _housingContext = housingContext;
        }
        ~ScraperForAruodas()
        {
            _scraperClient.EndScrape().Wait();
        }

        private async Task EndScrape()
        {
            await _scraperClient.EndScrape();
        }

        public async Task<Scrape> EnrichNewHousingsWithDetails()
        {
            var housingList = await _housingContext.Housings.Where(h => !String.IsNullOrEmpty(h.Link) && String.IsNullOrEmpty(h.AnketosKodas)).ToListAsync();

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
            var result = new Scrape { Message = "Enriching housing details finished successfully.", DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Aruodas, ScrapeType = ScrapeType.Full, ScrapeStatus = ScrapeStatus.Success, TotalQueries = _scraperClient.TotalQueries, NewQueries = _scraperClient.NewQueries, ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate };

            await EndScrape();

            return result;
        }
        public async Task<Scrape> FindRecentHousingPosts()
        {
            _scraperClient.ScrapeStartDate = DateTime.Now;

            for (int h = 0; h < 4; h++)
            {
                string siteEndpoint = h switch
                {
                    0 => "namai",
                    1 => "namu-nuoma",
                    2 => "butai",
                    3 => "butu-nuoma",
                    _ => "namai"
                };

                Debug.WriteLine($"[{DateTimeOffset.Now}] Running FindApartaments for Aruodas with {siteEndpoint} endpoint");

                string url = $"https://m.aruodas.lt/{siteEndpoint}/?FOrder=AddDate/";
                string pageUrl = url;
                int pageCount = 1;

                for (int i = 1; i <= 1; i++)
                {
                    Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                    if (i > 1)
                        pageUrl = url + $"puslapis/{i}/";

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

                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(pageUrl);
                        html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"[{DateTimeOffset.Now}]PuppeteerSharp failed to get {pageUrl}");
                        return new Scrape { Message = "PuppeteerSharp failed to get " + pageUrl, ScrapeStatus = ScrapeStatus.Failed, DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Aruodas, ScrapeType = ScrapeType.Partial };
                    }

                    doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    if (pageCount == 1)
                    {
                        var pageSelect = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("page-select-v2")).FirstOrDefault();

                        var node = pageSelect!.Descendants("a").FirstOrDefault()!.InnerText;

                        pageCount = Convert.ToInt32(node.Remove(0, 21).Trim());
                    }

                    var abstractList = doc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("search-result-list-big_thumbs")).ToList();

                    var listOfSelections = abstractList[0].Descendants("li").ToList();

                    foreach (var item in listOfSelections)
                    {

                        var tip = item.Descendants("a")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("result-item-info-container-big_thumbs"))
                                .FirstOrDefault();

                        if (tip != null)
                        {
                            var text = "https://m.aruodas.lt" + tip.GetAttributeValue("href", "");
                            _scraperClient.TotalQueries++;

                            var existingHousing = await _housingContext.Housings.FirstOrDefaultAsync(h => h.Link == text);

                            if (existingHousing == null)
                            {
                                try
                                {
                                    await SaveResult(true, text);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {text}");
                                }
                            }
                            else
                            {
                                _scraperClient.ScrapeEndDate = DateTime.Now;
                                var r = new Scrape { Message = "Recent post scraping finished successfully.", DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Aruodas, ScrapeType = ScrapeType.Partial, ScrapeStatus = ScrapeStatus.Success, TotalQueries = _scraperClient.TotalQueries, NewQueries = _scraperClient.NewQueries, ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate };

                                await EndScrape();

                                return r;
                            }
                        }
                    }
                }
            }
            _scraperClient.ScrapeEndDate = DateTime.Now;
            var result = new Scrape { Message = "Recent post scraping finished successfully.", DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Aruodas, ScrapeType = ScrapeType.Partial, ScrapeStatus = ScrapeStatus.Success, TotalQueries = _scraperClient.TotalQueries, NewQueries = _scraperClient.NewQueries, ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate };

            await EndScrape();

            return result;
        }
        private async Task GetHousingDetails(string url, string bustoTipas)
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
                    await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetCookiesAsync("https://m.aruodas.lt/");
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
                    .Equals("advert-info-header")).ToList();

            string title = "N/A";
            double price = 0;

            if (header.Count == 0)
            {
                header = doc.DocumentNode.Descendants("div")
                   .Where(node => node.GetAttributeValue("class", "")
                    .Equals("advert-info-header  in-project ")).ToList();

                title = header[0].Descendants("h1").FirstOrDefault()!.InnerText.Trim();
                price = Convert.ToDouble(header[0].Descendants("span").Where(node => node.GetAttributeValue("class", "")
                .Equals("main-price")).FirstOrDefault()!.InnerText.TrimEnd('€').Trim().Replace(" ", ""));
            }
            else
            {
                try
                {
                    title = header[0].Descendants("h1").FirstOrDefault()!.InnerText.Trim();
                    price = Convert.ToDouble(header[0].Descendants("span").Where(node => node.GetAttributeValue("class", "")
                    .Equals("main-price")).FirstOrDefault()!.InnerText.TrimEnd('€').Trim().Replace(" ", ""));
                }
                catch
                {
                    try
                    {
                        header = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("advert-info-header  in-project ")).ToList();

                        title = header[0].Descendants("h1").FirstOrDefault()!.InnerText.Trim();
                        price = Convert.ToDouble(header[0].Descendants("span").Where(node => node.GetAttributeValue("class", "")
                            .Equals("main-price")).FirstOrDefault()!.InnerText.TrimEnd('€').Trim().Replace(" ", ""));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting price for {url}");
                        return;
                    }
                }
            }

            house.Link = url;
            house.Title = title;
            house.Kaina = price;
            house.BustoTipas = bustoTipas;
            house.AnketosKodas = Regex.Match(url, @"\d{5,7}").Value;
            house.Aprasymas = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("collapsedText")).FirstOrDefault()!.InnerText;

            var abst = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("object-info ")).ToList();

            if (abst.Count == 0)
                abst = doc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("object-info with-rentdanger")).ToList();

            var descriptionList = abst[0].Descendants("dl").ToList();
            var descriptionTitle = descriptionList[0].Descendants("dt").ToList();
            var descriptionDefinition = descriptionList[0].Descendants("dd").ToList();

            for (int i = 0; i < descriptionTitle.Count; i++)
            {
                var dscTitle = descriptionTitle[i].InnerText;
                var dscDefinition = descriptionDefinition[i].InnerText;

                dscDefinition = Regex.Replace(dscDefinition, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

                switch (dscTitle)
                {
                    case "Kaina mėn.":
                        {
                            dscDefinition = Regex.Replace(dscDefinition, @"\s+", "");
                            dscDefinition = Regex.Replace(dscDefinition, @"€", "");
                            house.KainaMen = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Namo numeris":
                        {
                            house.NamoNumeris = dscDefinition;
                        }
                        break;
                    case "Buto numeris":
                        {
                            house.ButoNumeris = dscDefinition;
                        }
                        break;
                    case "Kambarių sk.":
                        {
                            house.KambariuSk = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Plotas":
                        {
                            dscDefinition = dscDefinition.Replace('²', ' ').Replace('m', ' ').Trim();
                            house.Plotas = Convert.ToDouble(dscDefinition);
                        }
                        break;
                    case "Aukštas":
                        {
                            house.Aukstas = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Aukštų sk.":
                        {
                            house.AukstuSk = Convert.ToInt32(dscDefinition);
                        }
                        break;
                    case "Metai":
                        {
                            if (Regex.IsMatch(dscDefinition, @"\d{4} renovacija"))
                            {
                                dscDefinition = Regex.Match(dscDefinition, @"\d{4} renovacija").Value;
                                dscDefinition = Regex.Match(dscDefinition, @"\d{4}").Value;
                                house.Metai = Convert.ToInt32(dscDefinition);
                            }
                            else
                                house.Metai = Convert.ToInt32(dscDefinition);

                        }
                        break;
                    case "Įrengimas":
                        {
                            house.Irengimas = dscDefinition;
                        }
                        break;
                    case "Pastato tipas":
                        {
                            house.PastatoTipas = dscDefinition;
                        }
                        break;
                    case "Šildymas":
                        {
                            house.Sildymas = dscDefinition;
                        }
                        break;
                    case "Pastato energijos suvartojimo klasė":
                        {
                            house.PastatoEnergijosSuvartojimoKlase = dscDefinition;
                        }
                        break;
                    case "Ypatybės":
                        {

                            dscDefinition = String.Join('/', descriptionDefinition[i].Descendants("span")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("special-comma")).Select(x => x.InnerHtml.Trim()).ToList());
                            house.Ypatybes = dscDefinition;
                        }
                        break;
                    case "Papildomos patalpos":
                        {
                            dscDefinition = String.Join('/', descriptionDefinition[i].Descendants("span")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("special-comma")).Select(x => x.InnerHtml.Trim()).ToList());
                            house.PapildomosPatalpos = dscDefinition;
                        }
                        break;
                    case "Papildoma įranga":
                        {
                            dscDefinition = String.Join('/', descriptionDefinition[i].Descendants("span")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("special-comma")).Select(x => x.InnerHtml.Trim()).ToList());
                            house.PapildomaIranga = dscDefinition;
                        }
                        break;
                    case "Apsauga":
                        {
                            dscDefinition = String.Join('/', descriptionDefinition[i].Descendants("span")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("special-comma")).Select(x => x.InnerHtml.Trim()).ToList());
                            house.Apsauga = dscDefinition;
                        }
                        break;
                    case "Vanduo":
                        {
                            house.Vanduo = dscDefinition;
                        }
                        break;
                    case "Iki vandens telkinio (m)":
                        {
                            house.IkiTelkinio = Convert.ToInt32(dscDefinition.Replace("m", "").Replace(" ", ""));
                        }
                        break;
                    case "Unikalus daikto numeris (RC numeris)":
                        {
                            house.RCNumeris = dscDefinition;
                        }
                        break;
                    case "Namo tipas":
                        {
                            house.NamoTipas = dscDefinition;
                        }
                        break;
                    case "Sklypo plotas":
                        {
                            house.SklypoPlotas = dscDefinition;
                        }
                        break;
                    case "Artimiausias vandens telkinys":
                        {
                            house.ArtimiausiasTelkinys = dscDefinition;
                        }
                        break;
                    default:
                        {
                            Debug.WriteLine($"Title not defined: {dscTitle}");
                            Thread.Sleep(1000);
                        }
                        break;
                }
            }

            await SaveResult(true, house);
            return;
        }
        public async Task<Scrape> FindAllHousingPosts()
        {
            _scraperClient.ScrapeStartDate = DateTime.Now;

            for (int h = 0; h < 4; h++)
            {
                string siteEndpoint = h switch
                {
                    0 => "namai",
                    1 => "namu-nuoma",
                    2 => "butai",
                    3 => "butu-nuoma",
                    _ => "namai"
                };

                Debug.WriteLine($"[{DateTimeOffset.Now}] Running FindApartaments for Aruodas with {siteEndpoint} endpoint");

                string url = $"https://m.aruodas.lt/{siteEndpoint}/";
                string pageUrl = url;
                int pageCount = 1;

                for (int i = 1; i <= 1; i++) // i<=should be pageCount
                {
                    Thread.Sleep((int)_scraperClient.SpeedLimit!); //Stopping to not get flagged as a robot

                    if (i > 1)
                        pageUrl = url + $"puslapis/{i}/";

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

                        await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GoToAsync(pageUrl);
                        html = await _scraperClient.NetworkPuppeteerClient.PuppeteerPage!.GetContentAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"[{DateTimeOffset.Now}]PuppeteerSharp failed to get {pageUrl}");
                        await EndScrape();
                        return new Scrape { Message = "PuppeteerSharp failed to get " + pageUrl, ScrapeStatus = ScrapeStatus.Failed, DateScraped = DateTime.Now, ScrapedSite = ScrapedSite.Aruodas, ScrapeType = ScrapeType.Full };
                    }

                    doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    if (pageCount == 1)
                    {
                        var pageSelect = doc.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("page-select-v2")).FirstOrDefault();

                        var node = pageSelect!.Descendants("a").FirstOrDefault()!.InnerText;

                        pageCount = Convert.ToInt32(node.Remove(0, 21).Trim());
                    }

                    var abstractList = doc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("search-result-list-big_thumbs")).ToList();

                    var listOfSelections = abstractList[0].Descendants("li").ToList();

                    foreach (var item in listOfSelections)
                    {

                        var tip = item.Descendants("a")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("result-item-info-container-big_thumbs"))
                                .FirstOrDefault();

                        if (tip != null)
                        {
                            var text = "https://m.aruodas.lt" + tip.GetAttributeValue("href", "");

                            try
                            {
                                await SaveResult(true, text);
                                _scraperClient.TotalQueries++;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"[{DateTimeOffset.Now}] Error while getting details for {text}");
                            }
                        }
                    }

                    Debug.WriteLine($"[{DateTimeOffset.Now}] <{i}/{pageCount}> Total queries made so far: {_scraperClient.TotalQueries} in /{siteEndpoint}/");
                }
            }
            _scraperClient.ScrapeEndDate = DateTime.Now;
            var result = new Scrape { Message = "Full scraping finished successfully.", ScrapeStatus = ScrapeStatus.Success, TotalQueries = _scraperClient.TotalQueries, NewQueries = _scraperClient.NewQueries, ScrapeTime = _scraperClient.ScrapeEndDate - _scraperClient.ScrapeStartDate };

            await EndScrape();

            return result;
        }
        private async Task SaveResult(bool savingDto, object result)
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
                            BustoTipas = text!.Remove(0, 21) switch
                            {
                                string s when s.StartsWith("butu-nuoma") => "butu-nuoma",
                                string s when s.StartsWith("namu-nuoma") => "namu-nuoma",
                                string s when s.StartsWith("butai") => "butai",
                                string s when s.StartsWith("namai") => "namai",
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