using System.Data.SqlTypes;
using System.Net;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using PuppeteerSharp;
using System.Diagnostics;

namespace HouseSpotter.Server.Utils
{
    public class ScraperClient
    {
        private readonly IConfiguration _configuration;

        ~ScraperClient()
        {
            EndScrape().Wait();
        }
        public ScraperClient(IConfiguration configuration)
        {
            _configuration = configuration;

            this.SpeedLimit = _configuration.GetValue<int?>("Scraper:SpeedLimit") ?? 100;

            NetworkHttpClient = new NetworkHttpClient();
            NetworkPuppeteerClient = new NetworkPuppeteerClient();
        }

        public async Task EndScrape()
        {
            Debug.WriteLine($"[{DateTimeOffset.Now}] Garbage collecting...");

            TotalQueries = 0;
            NewQueries = 0;

            if (NetworkHttpClient != null)
            {
                await NetworkHttpClient.Destroy();
            }
            if(NetworkPuppeteerClient != null)
            {
                await NetworkPuppeteerClient.Destroy();
            }
            GC.Collect();
            
            Debug.WriteLine($"[{DateTimeOffset.Now}] Garbage collected");
        }

        public bool UsingSql = false;
        public NetworkHttpClient NetworkHttpClient;
        public NetworkPuppeteerClient NetworkPuppeteerClient;
        public int NewQueries;
        public int TotalQueries;
        public int? SpeedLimit;
        public DateTime ScrapeStartDate;
        public DateTime ScrapeEndDate;
        public bool SkipPage = false;
    }
}
