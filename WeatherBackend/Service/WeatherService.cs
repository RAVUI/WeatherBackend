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

        
        public async Task<List<SimpleForecast>?> GetWeatherForecastByCity(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

            var response = await _httpClient.GetFromJsonAsync<WeatherForecastResponse>(url);
            if (response == null) return null;

            
            var groupedForecast = response.List
                .GroupBy(f => DateTime.Parse(f.Dt_txt).ToString("yyyy-MM-dd")) 
                .Select(g => new SimpleForecast
                {
                    Date = g.Key,  
                    Temperature = $"{g.First().Main.Temp}°C", 
                    Weather = g.First().Weather[0].Description
                })
                .ToList();

            return groupedForecast;
        }
    }
}
