
namespace Task3.Interfaces
{
    public interface IWeatherInfo
    {
        public string Temperature { get; }
        public string TemperatureF { get; }
        public string CloudCover { get; }
        public string Humidity { get; }
        public string RainIntensity { get; }
        public string SnowIntensity { get; }
        public string FreezingIntensity { get; }
        public string WindDirection { get; }
        public string WindSpeed { get; }
    }
}