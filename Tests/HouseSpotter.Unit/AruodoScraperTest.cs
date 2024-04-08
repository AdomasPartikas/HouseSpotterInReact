namespace HouseSpotter.Unit.Tests
{
    public class ScraperForAruodasTests : IClassFixture<ScraperForAruodasFixture>
    {
        private readonly ScraperForAruodas _scraperForAruodas;
        private readonly HousingContext _mockedHousingContext;
        private readonly ScraperClient _scraperClient;

        public ScraperForAruodasTests(ScraperForAruodasFixture fixture)
        {
            _scraperForAruodas = fixture.scraperForAruodas;
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
                                      .ReturnsAsync(staticFixtures.AruodasPositiveHtml);

            _scraperClient.NetworkPuppeteerClient = mockNetworkPuppeteerClient.Object;

            // Act
            var result = await _scraperForAruodas.FindAllHousingPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Full scraping finished successfully.", result.Message);
        }
    }
}