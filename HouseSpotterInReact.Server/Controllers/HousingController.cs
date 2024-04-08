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
    /// <summary>
    /// Controller for housing scraping operations.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("housespotter/scrapers")]
    public class HousingScraperController : ControllerBase
    {
        private HousingContext _housingContext;
        private readonly ScraperForAruodas _scraperForAruodas;
        private readonly ScraperForSkelbiu _scraperForSkelbiu;
        private readonly ScraperForDomo _scraperForDomo;
        
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HousingScraperController"/> class.
        /// </summary>
        /// <param name="housingContext"></param>
        /// <param name="scraperForAruodas"></param>
        /// <param name="mapper"></param>
        public HousingScraperController(HousingContext housingContext, ScraperForAruodas scraperForAruodas, ScraperForDomo scraperForDomo, ScraperForSkelbiu scraperForSkelbiu, IMapper mapper)
        {
            _scraperForAruodas = scraperForAruodas;
            _scraperForDomo = scraperForDomo;
            _scraperForSkelbiu = scraperForSkelbiu;
            _housingContext = housingContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Finds all housing posts from Aruodas website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
        [HttpPost("domo/findhousing/all")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DomoFindAllHousingPosts()
        {
            try
            {
                var result = await _scraperForDomo.FindAllHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Finds recent housing posts from Aruodas website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
        [HttpPost("domo/findhousing/recent")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DomoFindRecentHousingPosts()
        {
            try
            {
                var result = await _scraperForDomo.FindRecentHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("domo/enrichhousing")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DomoEnrichHousing()
        {
            try
            {
                var result = await _scraperForDomo.EnrichNewHousingsWithDetails();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Finds all housing posts from Aruodas website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
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

        /// <summary>
        /// Finds all housing posts from Skelbiu website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
        [HttpPost("skelbiu/findhousing/all")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SkelbiuFindAllHousingPosts()
        {
            try
            {
                var result = await _scraperForSkelbiu.FindAllHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Finds recent housing posts from Aruodas website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
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

        /// <summary>
        /// Finds recent housing posts from Skelbiu website.
        /// </summary>
        /// <returns>The scraped housing data.</returns>
        [HttpPost("skelbiu/findhousing/recent")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SkelbiuFindRecentHousingPosts()
        {
            try
            {
                var result = await _scraperForSkelbiu.FindRecentHousingPosts();
                _housingContext.Scrapes.Add(result);
                _housingContext.SaveChanges();

                return Ok(_mapper.Map<ScrapeDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Enriches new housing posts with additional details from Aruodas website.
        /// </summary>
        /// <returns>The enriched housing data.</returns>
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

        /// <summary>
        /// Enriches new housing posts with additional details from Skelbiu website.
        /// </summary>
        /// <returns>The enriched housing data.</returns>
        [HttpPost("skelbiu/enrichhousing")]
        [ProducesResponseType<ScrapeDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SkelbiuEnrichHousing()
        {
            try
            {
                var result = await _scraperForSkelbiu.EnrichNewHousingsWithDetails();
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

    /// <summary>
    /// Controller for housing database operations.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("housespotter/db")]
    public class HousingDbController : ControllerBase
    {
        private HousingContext _housingContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="HousingDbController"/> class.
        /// </summary>
        /// <param name="housingContext"></param>
        public HousingDbController(HousingContext housingContext)
        {
            _housingContext = housingContext;
        }

        /// <summary>
        /// Gets all housing data from the database.
        /// </summary>
        /// <returns>The list of housing data.</returns>
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

        /// <summary>
        /// Logs in a user with the provided credentials.
        /// </summary>
        /// <param name="user">The user login information.</param>
        /// <returns>The logged in user.</returns>
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

        /// <summary>
        /// Registers a new user with the provided information.
        /// </summary>
        /// <param name="body">The user registration information.</param>
        /// <returns>The registered user.</returns>
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

        /// <summary>
        /// Gets the saved searches of a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The list of saved searches.</returns>
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
                    return NotFound("Given user ID does not exist.");
                }
                if(user.SavedSearches == null)
                {
                    return Ok(new List<Housing>());
                }

                var result = await _housingContext.Housings.Where(h => user.SavedSearches.Any(u => u == h.ID.ToString())).ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Saves a search for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="search">The search to save.</param>
        /// <returns>The result of the save operation.</returns>
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
                    return NotFound("Given user ID does not exist.");
                }

                if(user.SavedSearches == null)
                {
                    user.SavedSearches = new List<string>();
                }

                if(user.SavedSearches.Contains(search))
                {
                    return Ok(user);
                }

                var housing = await _housingContext.Housings.Where(h => h.ID.ToString() == search).FirstOrDefaultAsync();

                if(housing == null)
                {
                    return NotFound("Given housing ID does not exist.");
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

        /// <summary>
        /// Removes a saved search for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="search">The search to remove.</param>
        /// <returns>The result of the remove operation.</returns>
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
                    return NotFound("Given user ID does not exist.");
                }

                if(user.SavedSearches == null)
                {
                    return Ok(user);
                }

                if(!user.SavedSearches.Contains(search))
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