
namespace AskBot
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("# AskBot here, welcome!");
      IpApi ip = IpApiClient.FetchIp();
      if (args.Length > 0)
      {
        if (args[0].ToLower() == "weather")
        {
          Console.WriteLine("## Fetching weather information...");
          var weather = new Weather();
          var weatherInfo = weather.FetchWeather();
          Console.WriteLine(weatherInfo);
        }
        else
        {
          PrintHelpMessage();
        }
      }
      else
      {
        PrintHelpMessage();
      }
      Console.WriteLine("Bye!");
      Console.Read();
    }

    private static void PrintHelpMessage()
    {
      Console.WriteLine("## Available commands:");
      Console.WriteLine("  - `weather` :  Fetch the current weather information");
    }
  }
}

