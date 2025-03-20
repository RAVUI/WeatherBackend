
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherBackend.Service;
using WeatherBackend.Models;

namespace WeatherBackend.Controllers
{
    [Route("api/v{version:apiVersion}/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

       
        [HttpGet("search")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetWeatherByCity([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new { message = "City name is required." });
            }

            var weatherData = await _weatherService.GetWeatherByCity(city);
            if (weatherData == null)
            {
                return NotFound(new { message = $"Weather data not available for city: {city}." });
            }

            return Ok(weatherData);
        }

       
        [HttpGet("forecast")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetWeatherForecast([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new { message = "City name is required." });
            }

            var forecast = await _weatherService.GetWeatherForecastByCity(city);
            if (forecast == null || !forecast.Any())
            {
                return NotFound(new { message = $"Weather forecast not available for city: {city}." });
            }

            return Ok(forecast);
        }

        
        [HttpGet("current-location")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetWeatherByLocation([FromQuery] double lat, [FromQuery] double lon)
        {
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {
                return BadRequest(new { message = "Invalid coordinates. Latitude must be between -90 and 90, Longitude between -180 and 180." });
            }

            var weatherData = await _weatherService.GetWeatherByCoordinates(lat, lon);
            if (weatherData == null)
            {
                return NotFound(new { message = "Weather data not available for the specified coordinates." });
            }

            return Ok(weatherData);
        }
    }
}