using HouseSpotter.Server.Models;
using HouseSpotter.Server.Scrapers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HouseSpotter.Server.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("housespotter/scrapers")]
    [ProducesResponseType(200)]
    public class HousingController : ControllerBase
    {
        private readonly ScraperForAruodas _scraperForAruodas;

        public HousingController(ScraperForAruodas scraperForAruodas)
        {
            _scraperForAruodas = scraperForAruodas;
        }

    [HttpPost("aruodas/findhousing/all")]
    public async Task<IActionResult> AruodasFindAllHousingPosts()
    {
        try
        {
            var result = await _scraperForAruodas.FindAllHousingPosts();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpPost("aruodas/findhousing/recent")]
    public async Task<IActionResult> AruodasFindRecentHousingPosts()
    {
        try
        {
            var result = await _scraperForAruodas.FindRecentHousingPosts();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpPost("aruodas/enrichhousing")]
    public async Task<IActionResult> AruodasEnrichHousing()
    {
        try
        {
            var result = await _scraperForAruodas.EnrichNewHousingsWithDetails();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }
    }
}