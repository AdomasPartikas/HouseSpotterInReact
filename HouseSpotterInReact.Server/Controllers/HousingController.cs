using AutoMapper;
using HouseSpotter.Server.Context;
using HouseSpotter.Server.Models;
using HouseSpotter.Server.Models.DTO;
using HouseSpotter.Server.Scrapers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HouseSpotter.Server.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("housespotter/scrapers")]
    public class HousingScraperController : ControllerBase
    {
        private HousingContext _housingContext;
        private readonly ScraperForAruodas _scraperForAruodas;
        private readonly IMapper _mapper;

        public HousingScraperController(HousingContext housingContext, ScraperForAruodas scraperForAruodas, IMapper mapper)
        {
            _scraperForAruodas = scraperForAruodas;
            _housingContext = housingContext;
            _mapper = mapper;
        }

        [HttpPost("aruodas/findhousing/all")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AruodasFindAllHousingPosts()
        {
            try
            {
                var result = await _scraperForAruodas.FindAllHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("aruodas/findhousing/recent")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AruodasFindRecentHousingPosts()
        {
            try
            {
                var result = await _scraperForAruodas.FindRecentHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("aruodas/enrichhousing")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AruodasEnrichHousing()
        {
            try
            {
                var result = await _scraperForAruodas.EnrichNewHousingsWithDetails();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }

    [ApiController]
    [Produces("application/json")]
    [Route("housespotter/db")]
    public class HousingDbController : ControllerBase
    {
        private HousingContext _housingContext;
        public HousingDbController(HousingContext housingContext)
        {
            _housingContext = housingContext;
        }

        [HttpGet("getallhousing")]
        [ProducesResponseType<Housing>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllHousing()
        {
            try
            {
                var result = await _housingContext.Housings.ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost("user/login")]
        [ProducesResponseType<User>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginBody user)
        {
            try
            {
                var result = await _housingContext.Users.Where(u => u.Username == user.Username && u.Password == user.Password).FirstOrDefaultAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost("user/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterBody body)
        {
            try
            {
                var user = new User
                {
                    Username = body.Username,
                    Password = body.Password,
                    Email = body.Email,
                    PhoneNumber = body.PhoneNumber,
                    CreatedAt = DateTime.Now,
                    SavedSearches = new List<string>(),
                    IsAdmin = body.IsAdmin
                };

                var result = await _housingContext.Users.AddAsync(user);
                await _housingContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpGet("user/{id}/savedSearches")]
        [ProducesResponseType<List<Housing>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSavedSearches(int id)
        {
            try
            {
                var user = await _housingContext.Users.Where(u => u.ID == id).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }
                if(user.SavedSearches == null)
                {
                    return Ok(new List<Housing>());
                }

                var result = await _housingContext.Housings.Where(h => user.SavedSearches.Any(u => u == h.AnketosKodas)).ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost("user/{id}/saveSearch")]
        [ProducesResponseType<User>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveSearch(int id, [FromBody] string search)
        {
            try
            {
                var user = await _housingContext.Users.Where(u => u.ID == id).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                if(user.SavedSearches == null)
                {
                    user.SavedSearches = new List<string>();
                }

                user.SavedSearches.Add(search);

                var result = await _housingContext.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpDelete("user/{id}/removeSearch")]
        [ProducesResponseType<User>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveSearch(int id, [FromBody] string search)
        {
            try
            {
                var user = await _housingContext.Users.Where(u => u.ID == id).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                if(user.SavedSearches == null)
                {
                    return Ok(user);
                }

                user.SavedSearches.Remove(search);

                var result = await _housingContext.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}