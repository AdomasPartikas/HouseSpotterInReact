
namespace HouseSpotter.Integration.Utils
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<HousingContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryHousingDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<HousingContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();
                }
            });
        }
    }
}