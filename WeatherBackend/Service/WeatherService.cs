using System.Net.Http.Json;
using WeatherBackend.Models;
using System.Globalization;

namespace WeatherBackend.Service
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ad350cf772d14e5d2828347ecd1a76bf";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Fetch current weather by city name
        public async Task<WeatherModel?> GetWeatherByCity(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

            var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);
            if (response == null) return null;

            return new WeatherModel
            {
                City = response.Name,
                Temperature = $"{response.Main.Temp}°C",
                Weather = response.Weather[0].Description,
                WindSpeed = response.Wind.Speed,
                Humidity = response.Main.Humidity,
            };
        }

        // Fetch current weather by coordinates
        public async Task<WeatherModel?> GetWeatherByCoordinates(double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";

            var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);
            if (response == null) return null;

            return new WeatherModel
            {
                City = response.Name,
                Temperature = $"{response.Main.Temp}°C",
                Weather = response.Weather[0].Description,
                WindSpeed = response.Wind.Speed,
                Humidity = response.Main.Humidity
            };
        }

        // ✅ Fetch 5-day weather forecast by city (returns only DATE and forecast)
        public async Task<List<SimpleForecast>?> GetWeatherForecastByCity(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

            var response = await _httpClient.GetFromJsonAsync<WeatherForecastResponse>(url);
            if (response == null) return null;

            // Convert forecast items to a simplified list with only Date & Forecast
            var groupedForecast = response.List
                .GroupBy(f => DateTime.Parse(f.Dt_txt).ToString("yyyy-MM-dd")) // Group by date only
                .Select(g => new SimpleForecast
                {
                    Date = g.Key,  // YYYY-MM-DD format
                    Temperature = $"{g.First().Main.Temp}°C", // Pick first forecast entry for the day
                    Weather = g.First().Weather[0].Description
                })
                .ToList();

            return groupedForecast;
        }
    }

    public class OpenWeatherResponse
    {
        public string Name { get; set; } = string.Empty;
        public WeatherInfo[] Weather { get; set; } = Array.Empty<WeatherInfo>();
        public MainInfo Main { get; set; } = new();
        public WindInfo Wind { get; set; } = new();
    }

    public class WeatherInfo { public string Description { get; set; } = string.Empty; }
    public class MainInfo { public float Temp { get; set; } public int Humidity { get; set; } }
    public class WindInfo { public float Speed { get; set; } }

    // ✅ New simplified forecast model
    public class WeatherForecastResponse
    {
        public List<ForecastItem> List { get; set; } = new();
    }

    public class ForecastItem
    {
        public string Dt_txt { get; set; } = string.Empty;  // Original API date-time
        public MainInfo Main { get; set; } = new();
        public List<WeatherInfo> Weather { get; set; } = new();
    }

    // ✅ New class to return only Date & Forecast
    public class SimpleForecast
    {
        public string Date { get; set; } = string.Empty;  // YYYY-MM-DD format
        public string Temperature { get; set; } = string.Empty;
        public string Weather { get; set; } = string.Empty;
    }
}
