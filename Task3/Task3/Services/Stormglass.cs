using Task3.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Task3.Services
{
    public class Stormglass : IService
    {
        private readonly HttpClient client;
        private readonly IEnv env;
        public Stormglass(HttpClient client, IEnv env)
        {
            this.client = client;
            this.env = env;
        }
        public string Name => "stormglass.io";

        public IWeatherInfo GetWeather()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            var requestString = $"https://api.stormglass.io/v2/weather/point?" +
            string.Join("&",
            [
                $"lat={env.Latitude}",
                $"lng={env.Longitude}",
                $"params=airTemperature,cloudCover,humidity,precipitation,windDirection,windSpeed",
                $"start={time}",
                $"end={time}",
                $"source=noaa"
            ]);

            var request = new HttpRequestMessage(HttpMethod.Get, requestString);
            request.Headers.Authorization = new AuthenticationHeaderValue(env.StormglassApiKey);

            var response = client.SendAsync(request).Result.EnsureSuccessStatusCode();

            var content = response.Content;
            var body = content.ReadAsStringAsync().Result;
            var json = JsonDocument.Parse(body);

            var forecast = json.RootElement.GetProperty("hours").EnumerateArray().First();

            return new WeatherInfo
            {
                Temperature = forecast.GetProperty("airTemperature").GetProperty("noaa").GetRawText(),
                CloudCover = forecast.GetProperty("cloudCover").GetProperty("noaa").GetRawText(),
                Humidity = forecast.GetProperty("humidity").GetProperty("noaa").GetRawText(),
                RainIntensity = forecast.GetProperty("precipitation").GetProperty("noaa").GetRawText(),
                WindDirection = forecast.GetProperty("windDirection").GetProperty("noaa").GetRawText(),
                WindSpeed = forecast.GetProperty("windSpeed").GetProperty("noaa").GetRawText()
            };
        }
    }
}