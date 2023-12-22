
namespace Task3.Interfaces
{
    public interface IService
    {
        string Name { get; }
        IWeatherInfo GetWeather();
    }
}