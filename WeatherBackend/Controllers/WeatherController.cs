using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherBackend.Service;

namespace WeatherBackend.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // ✅ Fetch current weather by city name
        [HttpGet("search")]
        public async Task<IActionResult> GetWeatherByCity([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new { message = "City name is required." });
            }

            var weatherData = await _weatherService.GetWeatherByCity(city);
            if (weatherData == null)
            {
                return NotFound(new { message = "Weather data not available." });
            }

            return Ok(weatherData);
        }

        // ✅ Fetch 5-day weather forecast by city name
        [HttpGet("forecast")]
        public async Task<IActionResult> GetWeatherForecast([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new { message = "City name is required." });
            }

            var forecast = await _weatherService.GetWeatherForecastByCity(city);
            if (forecast == null)
            {
                return NotFound(new { message = "Weather forecast not available." });
            }

            return Ok(forecast);
        }

        // ✅ Fetch weather by geographic coordinates (latitude & longitude)
        [HttpGet("current-location")]
        public async Task<IActionResult> GetWeatherByLocation([FromQuery] double lat, [FromQuery] double lon)
        {
            if (lat == 0 || lon == 0)
            {
                return BadRequest(new { message = "Latitude and Longitude are required." });
            }

            var weatherData = await _weatherService.GetWeatherByCoordinates(lat, lon);
            if (weatherData == null)
            {
                return NotFound(new { message = "Weather data not available." });
            }

            return Ok(weatherData);
        }
    }
}
