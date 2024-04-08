namespace HouseSpotter.Unit.Fixtures
{
    public class ScraperForAruodasFixture : IDisposable
    {
        public ScraperForAruodas scraperForAruodas;
        public HousingContext mockedHousingContext;
        public ScraperClient scraperClient;

        public ScraperForAruodasFixture()
        {
            var loggerMock = new Mock<ILogger<ScraperForAruodas>>();

            var options = new DbContextOptionsBuilder<HousingContext>()
                .UseInMemoryDatabase(databaseName: "TestHousingDb")
                .Options;

            mockedHousingContext = new HousingContext(options);

            var inMemorySettings = new Dictionary<string, string> {
                {"Scraper:SpeedLimit", "100"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            scraperClient = new Mock<ScraperClient>(configuration).Object;

            scraperForAruodas = new ScraperForAruodas(scraperClient, loggerMock.Object, mockedHousingContext);
        }

        public void Dispose()
        {
            scraperClient.EndScrape().Wait();
            mockedHousingContext.Database.EnsureDeleted();
            mockedHousingContext.Dispose();
        }
    }
    public class ScraperForDomoFixture : IDisposable
    {
        public ScraperForDomo scraperForDomo;
        public HousingContext mockedHousingContext;
        public ScraperClient scraperClient;

        public ScraperForDomoFixture()
        {
            var loggerMock = new Mock<ILogger<ScraperForDomo>>();

            var options = new DbContextOptionsBuilder<HousingContext>()
                .UseInMemoryDatabase(databaseName: "TestHousingDb")
                .Options;

            mockedHousingContext = new HousingContext(options);

            var inMemorySettings = new Dictionary<string, string> {
                {"Scraper:SpeedLimit", "100"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            scraperClient = new Mock<ScraperClient>(configuration).Object;

            scraperForDomo = new ScraperForDomo(scraperClient, loggerMock.Object, mockedHousingContext);
        }

        public void Dispose()
        {
            scraperClient.EndScrape().Wait();
            mockedHousingContext.Database.EnsureDeleted();
            mockedHousingContext.Dispose();
        }
    }
    public class ScraperForSkelbiuFixture : IDisposable
    {
        public ScraperForSkelbiu scraperForSkelbiu;
        public HousingContext mockedHousingContext;
        public ScraperClient scraperClient;

        public ScraperForSkelbiuFixture()
        {
            var loggerMock = new Mock<ILogger<ScraperForSkelbiu>>();

            var options = new DbContextOptionsBuilder<HousingContext>()
                .UseInMemoryDatabase(databaseName: "TestHousingDb")
                .Options;

            mockedHousingContext = new HousingContext(options);

            mockedHousingContext.Database.EnsureCreated();

            var inMemorySettings = new Dictionary<string, string> {
                {"Scraper:SpeedLimit", "100"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            scraperClient = new Mock<ScraperClient>(configuration).Object;

            scraperForSkelbiu = new ScraperForSkelbiu(scraperClient, loggerMock.Object, mockedHousingContext);
        }

        public void Dispose()
        {
            scraperClient.EndScrape().Wait();
            mockedHousingContext.Database.EnsureDeleted();
            mockedHousingContext.Dispose();
        }
    }
}