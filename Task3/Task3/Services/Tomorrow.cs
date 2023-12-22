using Task3.Interfaces;
using System.Text.Json;

namespace Task3.Services
{
    public class Tomorrow : IService
    {
        private readonly HttpClient client;
        private readonly IEnv env;
        public Tomorrow(HttpClient client, IEnv env)
        {
            this.client = client;
            this.env = env;
        }
        public string Name => "tomorrow.io";
        public IWeatherInfo GetWeather()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            var request = $"https://api.tomorrow.io/v4/weather/forecast?" +
            string.Join("&",
            [
                $"location={env.Latitude},{env.Longitude}",
                $"apikey={env.TomorrowApiKey}"
            ]);

            var response = client.GetAsync(request).Result.EnsureSuccessStatusCode();

            var content = response.Content;
            var body = content.ReadAsStringAsync().Result;
            var json = JsonDocument.Parse(body);

            var forecast = json.RootElement.GetProperty("timelines").GetProperty("minutely").EnumerateArray().First().GetProperty("values");

            return new WeatherInfo
            {
                Temperature = forecast.GetProperty("temperature").GetRawText(),
                CloudCover = forecast.GetProperty("cloudCover").GetRawText(),
                Humidity = forecast.GetProperty("humidity").GetRawText(),
                RainIntensity = forecast.GetProperty("rainIntensity").GetRawText(),
                WindDirection = forecast.GetProperty("windDirection").GetRawText(),
                WindSpeed = forecast.GetProperty("windSpeed").GetRawText(),
                SnowIntensity = forecast.GetProperty("snowIntensity").GetRawText(),
                FreezingIntensity = forecast.GetProperty("freezingRainIntensity").GetRawText(),
            };
        }
    }
}