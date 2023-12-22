
namespace Task3.Interfaces
{
    public interface IEnv
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public string StormglassApiKey { get; }
        public string TomorrowApiKey { get; }
    }
}