
namespace Task3.Interfaces
{
    public class WeatherInfo : IWeatherInfo
    {
        readonly string noData;
        string? temperature;
        string? clouds;
        string? humidity;
        string? rainIntensity;
        string? snowIntensity;
        string? freezingIntensity;
        string? windDirection;
        string? windSpeed;
        public WeatherInfo()
        {
            noData = "No data";
        }
        public WeatherInfo(string noData)
        {
            this.noData = noData;
        }
        string Check(string? data) => data ?? noData;
        public string Temperature { get => Check(temperature); set { temperature = value; } }
        public string TemperatureF
        {
            get
            {
                if (temperature is not null)
                {
                    return (32 + (int)(double.Parse(temperature) / 0.5556)).ToString();
                }
                return noData;
            }
            set
            {
                temperature = ((int)((double.Parse(value) - 32) / 0.5556)).ToString();
            }
        }
        public string CloudCover { get => Check(clouds); set { clouds = value; } }
        public string Humidity { get => Check(humidity); set { humidity = value; } }
        public string RainIntensity { get => Check(rainIntensity); set { rainIntensity = value; } }
        public string SnowIntensity { get => Check(snowIntensity); set { snowIntensity = value; } }
        public string FreezingIntensity { get => Check(freezingIntensity); set { freezingIntensity = value; } }
        public string WindDirection { get => Check(windDirection); set { windDirection = value; } }
        public string WindSpeed { get => Check(windSpeed); set { windSpeed = value; } }
    }
}