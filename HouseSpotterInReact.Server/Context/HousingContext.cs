using Microsoft.EntityFrameworkCore;
using HouseSpotter.Server.Models;

namespace HouseSpotter.Server.Context
{
    /// <summary>
    /// Represents the database context for the Housing application.
    /// </summary>
    public class HousingContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HousingContext"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public HousingContext(DbContextOptions<HousingContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet of Housing entities.
        /// </summary>
        public DbSet<Housing> Housings { get; set; }

        /// <summary>
        /// Gets or sets the DbSet of User entities.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the DbSet of Scrape entities.
        /// </summary>
        public DbSet<Scrape> Scrapes { get; set; }

        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Housing>().ToTable("Housing");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Scrape>().ToTable("Scrape");
        }
    }
}