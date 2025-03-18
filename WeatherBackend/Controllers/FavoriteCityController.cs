using Microsoft.AspNetCore.Mvc;
using WeatherBackend.Models;
using WeatherBackend.Service;

namespace WeatherBackend.Controllers
{
    [Route("api/favoritecities")]
    [ApiController]
    public class FavoriteCityController : ControllerBase
    {
        private readonly FavoriteCityService _favoriteCityService;

        public FavoriteCityController(FavoriteCityService favoriteCityService)
        {
            _favoriteCityService = favoriteCityService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<FavoriteCity>>> GetFavoriteCities(string userId)
        {
            try
            {
                var cities = await _favoriteCityService.GetFavoriteCities(userId);
                return Ok(cities);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<FavoriteCity>> AddFavoriteCity([FromBody] FavoriteCity city)
        {
            try
            {
                var addedCity = await _favoriteCityService.AddFavoriteCity(city);
                return CreatedAtAction(nameof(GetFavoriteCities), new { userId = city.UserId }, addedCity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{cityId}")]
        public async Task<IActionResult> RemoveFavoriteCity(string cityId, [FromQuery] string userId)
        {
            try
            {
                await _favoriteCityService.RemoveFavoriteCity(cityId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}