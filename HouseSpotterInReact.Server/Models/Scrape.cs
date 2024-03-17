using System;
using Google.Protobuf.WellKnownTypes;

namespace HouseSpotter.Server.Models
{
    public class Scrape
    {
        public int ID { get; set; }
        public ScrapeType ScrapeType { get; set; }
        public ScrapeStatus ScrapeStatus { get; set; }
        public ScrapedSite ScrapedSite { get; set; }
        public DateTime DateScraped { get; set; }
        public TimeSpan ScrapeTime { get; set; }
        public string? Message { get; set; }
        public int? TotalQueries { get; set; }
        public int? NewQueries { get; set; }
    }
}