using System;
using Google.Protobuf.WellKnownTypes;

namespace HouseSpotter.Server.Models
{
    /// <summary>
    /// Represents a scrape operation.
    /// </summary>
    public class Scrape
    {
        /// <summary>
        /// Gets or sets the ID of the scrape.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the type of the scrape.
        /// </summary>
        public ScrapeType ScrapeType { get; set; }

        /// <summary>
        /// Gets or sets the status of the scrape.
        /// </summary>
        public ScrapeStatus ScrapeStatus { get; set; }

        /// <summary>
        /// Gets or sets the site that was scraped.
        /// </summary>
        public ScrapedSite ScrapedSite { get; set; }

        /// <summary>
        /// Gets or sets the date when the scrape was performed.
        /// </summary>
        public DateTime DateScraped { get; set; }

        /// <summary>
        /// Gets or sets the duration of the scrape operation.
        /// </summary>
        public TimeSpan ScrapeTime { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the scrape.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the total number of queries performed during the scrape.
        /// </summary>
        public int? TotalQueries { get; set; }

        /// <summary>
        /// Gets or sets the number of new queries performed during the scrape.
        /// </summary>
        public int? NewQueries { get; set; }
    }
}