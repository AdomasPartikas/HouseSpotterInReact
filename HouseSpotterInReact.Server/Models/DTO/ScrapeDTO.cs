using System;

namespace HouseSpotter.Server.Models.DTO
{
    public class ScrapeDTO
    {
        public int ID { get; set; }
        public string? ScrapeType { get; set; }
        public string? ScrapeStatus { get; set; }
        public string? ScrapedSite { get; set; }
        public DateTime DateScraped { get; set; }
        public TimeSpan ScrapeTime { get; set; }
        public string? Message { get; set; }
        public int? TotalQueries { get; set; }
        public int? NewQueries { get; set; }
    }
}