using System;

namespace HouseSpotter.Server.Models
{
    public class ScrapeInformation
    {
        public bool ScrapeSucceded { get; set; }
        public string? Message { get; set; }
        public TimeSpan? ScrapeTime { get; set; }
        public int? TotalQueries { get; set; }
        public int? NewQueries { get; set; }
    }
}