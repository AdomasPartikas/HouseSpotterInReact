namespace HouseSpotter.Unit.Tests
{
    public class ScraperForSkelbiuTests : IClassFixture<ScraperForSkelbiuFixture>
    {
        private readonly ScraperForSkelbiu _scraperForSkelbiu;
        private readonly HousingContext _mockedHousingContext;
        private readonly ScraperClient _scraperClient;

        public ScraperForSkelbiuTests(ScraperForSkelbiuFixture fixture)
        {
            _scraperForSkelbiu = fixture.scraperForSkelbiu;
            _mockedHousingContext = fixture.mockedHousingContext;
            _scraperClient = fixture.scraperClient;
        }

        [Fact]
        public async Task FindAllHousingPosts_ShouldPopulateHousingContextWithNewPosts()
        {
            // Arrange
            var mockNetworkPuppeteerClient = new Mock<NetworkPuppeteerClient>();
            var staticFixtures = new StaticFixtures();

            mockNetworkPuppeteerClient.Setup(client => client.PuppeteerInitialized).Returns(true);
            mockNetworkPuppeteerClient.Setup(client => client.Initialize()).Returns(Task.CompletedTask);
            mockNetworkPuppeteerClient.Setup(client => client.PuppeteerPage!.GoToAsync(It.IsAny<string>(), null, null));

            mockNetworkPuppeteerClient.Setup(client => client.PuppeteerPage!.GetContentAsync())
                                      .ReturnsAsync(staticFixtures.SkelbiuPositiveHtml);

            _scraperClient.NetworkPuppeteerClient = mockNetworkPuppeteerClient.Object;

            // Act
            var result = await _scraperForSkelbiu.FindAllHousingPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Scrape finished successfully", result.Message);
        }
<<<<<<< HEAD

        [Fact]
        public async Task FindAllHousingPosts_ShouldHandleScrapingFailureGracefully()
        {
            // Arrange
            var mockNetworkPuppeteerClient = new Mock<NetworkPuppeteerClient>();
            var staticFixtures = new StaticFixtures();

            // Configure mock to throw an exception when Initialize is called
            mockNetworkPuppeteerClient.Setup(client => client.Initialize()).ThrowsAsync(new Exception("PuppeteerSharp failed to get https://m.sk"));

            _scraperClient.NetworkPuppeteerClient = mockNetworkPuppeteerClient.Object;

            // Act
            var result = await _scraperForSkelbiu.FindAllHousingPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ScrapeStatus.Failed, result.ScrapeStatus);
            Assert.Contains("PuppeteerSharp failed to get https://m.sk", result.Message);
        }



=======
>>>>>>> 6f67b59383035f226286a773186642b115071609
    }
}