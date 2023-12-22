using Microsoft.AspNetCore.Mvc;
using Task3.Interfaces;

namespace WebTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherInfoController : ControllerBase
    {
        private readonly Dictionary<string, IService> services;

        public WeatherInfoController(IEnumerable<IService> weatherServices)
        {
            services = [];
            foreach (var service in weatherServices)
            {
                services[service.Name] = service;
            }
        }

        [HttpGet(Name = "GetWeatherInfo")]
        public IWeatherInfo GetFromSingle([FromQuery] string serviceName) => services[serviceName].GetWeather();

        [HttpGet("ServicesNames")]
        public IActionResult GetServices() => Ok(services.Keys);

        [HttpGet("AllWeatherInfos")]
        public IEnumerable<IWeatherInfo> GetFromAll() => services.Select(x => x.Value.GetWeather()).ToList();
    }
}