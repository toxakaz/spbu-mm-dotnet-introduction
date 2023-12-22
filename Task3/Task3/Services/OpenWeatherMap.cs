using Task3.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Task3.Services
{
    public class OpenWeatherMap : IService
    {
        private readonly HttpClient client;
        private readonly IEnv env;
        public OpenWeatherMap(HttpClient client, IEnv env)
        {
            this.client = client;
            this.env = env;
        }
        public string Name => "openweathermap.org";

        public IWeatherInfo GetWeather()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            var requestString = $"https://api.openweathermap.org/data/2.5/weather?" +
            string.Join("&",
            [
               $"lat={env.Latitude}",
                 $"lon={env.Longitude}",
                $"appid={env.OpenWeatherMapApiKey}",
                "units=metric"
            ]);

            var request = new HttpRequestMessage(HttpMethod.Get, requestString);
            request.Headers.Add("accept", "application/json");

            var response = client.SendAsync(request).Result.EnsureSuccessStatusCode();

            var content = response.Content;
            var body = content.ReadAsStringAsync().Result;
            var json = JsonDocument.Parse(body);

            var main = json.RootElement.GetProperty("main");
            var clouds = json.RootElement.GetProperty("clouds");
            var wind = json.RootElement.GetProperty("wind");

            var result = new WeatherInfo()
            {
                Temperature = main.GetProperty("temp").GetRawText(),
                CloudCover = clouds.GetProperty("all").GetRawText(),
                Humidity = main.GetProperty("humidity").GetRawText(),
                WindDirection = wind.GetProperty("deg").GetRawText(),
                WindSpeed = wind.GetProperty("speed").GetRawText()
            };

            if (json.RootElement.TryGetProperty("rain", out var rain))
            {
                result.RainIntensity = rain.GetProperty("1h").GetRawText();
            }
            if (json.RootElement.TryGetProperty("snow", out var snow))
            {
                result.SnowIntensity = snow.GetProperty("1h").GetRawText();
            }

            return result;
        }
    }
}