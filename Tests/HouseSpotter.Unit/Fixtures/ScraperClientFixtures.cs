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

            mockedHousingContext.Housings.Add(new Housing
            {
                Link = "https://m.aruodas.lt/namai-siauliu-rajone-raizgiu-k-tvenkinio-g-parduodamas-namas-su-arais-zemes-sklypu-2-1599479/?return_url=%2Fnamai%2F%3Fobj%3D2",
                BustoTipas = "namas"
                // Add other properties as needed
            });

            mockedHousingContext.SaveChanges();

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

            mockedHousingContext.Housings.Add(new Housing
            {
                Link = "https://domoplius.lt/skelbimai/parduodamas-gyvenamasis-namas-vilniaus-rajono-sav-didzioji-riese-snieguoliu-g-8245597.html",
                BustoTipas = "namas"
                // Add other properties as needed
            });

            mockedHousingContext.SaveChanges();

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

            mockedHousingContext.Housings.Add(new Housing
            {
                Link = "https://www.skelbiu.lt/skelbimai/4-kamb-153-kv-miesto-komunik-12-86a-priduotas-73823587.html",
                BustoTipas = "namas"
                // Add other properties as needed
            });

            mockedHousingContext.SaveChanges();

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