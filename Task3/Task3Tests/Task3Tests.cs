using Moq;
using Moq.Protected;

using System.Net;
using Task3.Interfaces;
using Task3.Services;

namespace Task3Tests
{
    public class Tests
    {
        readonly Mock<IEnv> env = new();
        [SetUp]
        public void Setup()
        {
            env.Setup(x => x.Latitude).Returns(0);
            env.Setup(x => x.Longitude).Returns(0);
            env.Setup(x => x.TomorrowApiKey).Returns("apiKey");
            env.Setup(x => x.StormglassApiKey).Returns("apiKey");
        }
        [Test]
        public void TestStormglassRequest()
        {
            var mock = new Mock<HttpMessageHandler>();

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"
                    {
                        ""hours"": [
                            {
                                ""time"": ""2023-12-31T00:00:00+00:00"",
                                ""airTemperature"": {
                                    ""noaa"": 1
                                },
                                ""cloudCover"": {
                                    ""noaa"": 2
                                },
                                ""humidity"": {
                                    ""noaa"": 3
                                },
                                ""precipitation"": {
                                    ""noaa"": 4
                                },
                                ""windDirection"": {
                                    ""noaa"": 5
                                },
                                ""windSpeed"": {
                                    ""noaa"": 6
                                }
                            }
                        ]
                    }"
                )
            };

            mock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(response);

            var httpClient = new HttpClient(mock.Object);

            IService stormglass = new Stormglass(httpClient, env.Object);

            var weatherInfo = stormglass.GetWeather();

            Assert.That(weatherInfo, Is.Not.Null);
            Assert.That(weatherInfo.Temperature, Is.EqualTo("1"));
            Assert.That(weatherInfo.CloudCover, Is.EqualTo("2"));
            Assert.That(weatherInfo.Humidity, Is.EqualTo("3"));
            Assert.That(weatherInfo.RainIntensity, Is.EqualTo("4"));
            Assert.That(weatherInfo.SnowIntensity, Is.EqualTo("No data"));
            Assert.That(weatherInfo.FreezingIntensity, Is.EqualTo("No data"));
            Assert.That(weatherInfo.WindDirection, Is.EqualTo("5"));
            Assert.That(weatherInfo.WindSpeed, Is.EqualTo("6"));
        }

        [Test]
        public void TestTomorrowRequest()
        {
            var mock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"
                    {
                    ""timelines"": {
                            ""minutely"": [
                                {
                                    ""time"": ""2023-12-31T00:00:00+00:00"",
                                    ""values"": {
                                        ""temperature"": 1,
                                        ""cloudCover"": 2,
                                        ""humidity"": 3,
                                        ""rainIntensity"": 4,
                                        ""snowIntensity"": 5,
                                        ""freezingRainIntensity"": 6,
                                        ""windDirection"": 7,
                                        ""windSpeed"": 8
                                    }
                                }
                            ]
                        }
                    }"
                )
            };

            mock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(response);

            var httpClient = new HttpClient(mock.Object);

            IService stormglassService = new Tomorrow(httpClient, env.Object);

            var weatherInfo = stormglassService.GetWeather();

            Assert.That(weatherInfo, Is.Not.Null);
            Assert.That(weatherInfo.Temperature, Is.EqualTo("1"));
            Assert.That(weatherInfo.CloudCover, Is.EqualTo("2"));
            Assert.That(weatherInfo.Humidity, Is.EqualTo("3"));
            Assert.That(weatherInfo.RainIntensity, Is.EqualTo("4"));
            Assert.That(weatherInfo.SnowIntensity, Is.EqualTo("5"));
            Assert.That(weatherInfo.FreezingIntensity, Is.EqualTo("6"));
            Assert.That(weatherInfo.WindDirection, Is.EqualTo("7"));
            Assert.That(weatherInfo.WindSpeed, Is.EqualTo("8"));
        }
    }
}