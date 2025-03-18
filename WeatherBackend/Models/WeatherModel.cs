namespace WeatherBackend.Models
{
    public class WeatherModel
    {
        public string? City { get; set; }
        public string? Weather { get; set; }
        public string? Temperature { get; set; }
        public float WindSpeed { get; set; }
        public int Humidity { get; set; }

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


    public class WeatherForecastResponse
    {
        public List<ForecastItem> List { get; set; } = new();
    }

    public class ForecastItem
    {
        public string Dt_txt { get; set; } = string.Empty;
        public MainInfo Main { get; set; } = new();
        public List<WeatherInfo> Weather { get; set; } = new();
    }


    public class SimpleForecast
    {
        public string Date { get; set; } = string.Empty;
        public string Temperature { get; set; } = string.Empty;
        public string Weather { get; set; } = string.Empty;

    }

}
