public class HousingScraperControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public HousingScraperControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DomoFindAllHousingPosts_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/housespotter/scrapers/domo/findhousing/all", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scrapeDTO = JsonConvert.DeserializeObject<ScrapeDTO>(content);

        Assert.NotNull(scrapeDTO);
        Assert.Equal("Success", scrapeDTO.ScrapeStatus);
    }

    
    [Fact]
    public async Task AruodasFindAllHousingPosts_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/housespotter/scrapers/aruodas/findhousing/all", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scrapeDTO = JsonConvert.DeserializeObject<ScrapeDTO>(content);

        Assert.NotNull(scrapeDTO);
        Assert.Equal("Success", scrapeDTO.ScrapeStatus);
    }

    [Fact]
    public async Task SkelbiuFindAllHousingPosts_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/housespotter/scrapers/skelbiu/findhousing/all", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scrapeDTO = JsonConvert.DeserializeObject<ScrapeDTO>(content);

        Assert.NotNull(scrapeDTO);
        Assert.Equal("Success", scrapeDTO.ScrapeStatus);
    }

    // Other tests
}
