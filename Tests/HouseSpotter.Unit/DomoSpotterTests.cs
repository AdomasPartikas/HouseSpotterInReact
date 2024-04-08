namespace HouseSpotter.Unit.Tests
{
    public class ScraperForDomoTests : IClassFixture<ScraperForDomoFixture>
    {
        private readonly ScraperForDomo _scraperForDomo;
        private readonly HousingContext _mockedHousingContext;
        private readonly ScraperClient _scraperClient;

        public ScraperForDomoTests(ScraperForDomoFixture fixture)
        {
            _scraperForDomo = fixture.scraperForDomo;
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
                                      .ReturnsAsync(staticFixtures.DomoPositiveHtml);

            _scraperClient.NetworkPuppeteerClient = mockNetworkPuppeteerClient.Object;

            // Act
            var result = await _scraperForDomo.FindAllHousingPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Full scraping finished successfully.", result.Message);
        }

        [Fact]
        public async Task FindAllHousingPosts_ShouldHandleScrapingFailureGracefully()
        {
            // Arrange
            var mockNetworkPuppeteerClient = new Mock<NetworkPuppeteerClient>();
            var staticFixtures = new StaticFixtures();

            // Configure mock to throw an exception with the desired message when GetContentAsync is called
            mockNetworkPuppeteerClient.Setup(client => client.PuppeteerPage.GetContentAsync())
                                    .ThrowsAsync(new Exception("PuppeteerSharp failed to get https://domo"));

            _scraperClient.NetworkPuppeteerClient = mockNetworkPuppeteerClient.Object;

            // Act
            var result = await _scraperForDomo.FindAllHousingPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ScrapeStatus.Failed, result.ScrapeStatus);
            Assert.Contains("PuppeteerSharp failed to get https://domo", result.Message);
        }
        
        [Fact]
        public async Task EnrichNewHousingsWithDetails_ShouldUpdateHousingsWithCorrectDetails()
        {
            // Arrange
            var fixture = new ScraperForDomoFixture(); // Initialize the fixture

            // Act
            var result = await fixture.scraperForDomo.EnrichNewHousingsWithDetails(); // Enrich housings

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Enriching housing details finished successfully.", result.Message); // Check the result message

            // Clean up
            fixture.Dispose();
        }

    }
}