using Task3.Interfaces;

namespace Task3
{
    public class Env : IEnv
    {
        public double Latitude => double.Parse(GetEnvironmentVariable("Latitude"));
        public double Longitude => double.Parse(GetEnvironmentVariable("Longitude"));
        public string StormglassApiKey => GetEnvironmentVariable("StormglassApiKey");
        public string TomorrowApiKey => GetEnvironmentVariable("TomorrowApiKey");
        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Environment variable \"{name}\" not found");
        }
    }
}