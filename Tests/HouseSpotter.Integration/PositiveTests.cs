using HouseSpotter.Server.Models;

[CollectionDefinition("Postivity", DisableParallelization = true)]
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
        client.Timeout = TimeSpan.FromMinutes(5);

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
        client.Timeout = TimeSpan.FromMinutes(5);

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
        client.Timeout = TimeSpan.FromMinutes(5);

        // Act
        var response = await client.PostAsync("/housespotter/scrapers/skelbiu/findhousing/all", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scrapeDTO = JsonConvert.DeserializeObject<ScrapeDTO>(content);

        Assert.NotNull(scrapeDTO);
        Assert.Equal("Success", scrapeDTO.ScrapeStatus);
    }

    [Fact]
    public async Task Database_GetAllHousings()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(1);

        // Act
        var response = await client.GetAsync("/housespotter/db/getallhousing");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scrapeDTO = JsonConvert.DeserializeObject<List<Housing>>(content);

        Assert.NotNull(scrapeDTO);
    }
}
