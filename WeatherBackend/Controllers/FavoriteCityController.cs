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
        public async Task<IActionResult> GetFavoriteCities(string userId)
        {
            var cities = await _favoriteCityService.GetFavoriteCities(userId);
            return Ok(cities);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteCity([FromBody] FavoriteCity city)
        {
            await _favoriteCityService.AddFavoriteCity(city);
            return Ok();
        }

        [HttpDelete("{cityId}")]
        public async Task<IActionResult> RemoveFavoriteCity(string cityId)
        {
            await _favoriteCityService.RemoveFavoriteCity(cityId);
            return Ok();
        }
    }
}
