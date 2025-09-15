
namespace AskBot
{
  class Program
  {
    /// <summary>
    /// Entry point of the AskBot application.
    /// Greets the user, fetches and prints IP information, and optionally fetches weather information
    /// if the "weather" argument is provided. Displays a help message for other arguments or if no arguments are given.
    /// </summary>
    /// <param name="args">
    /// Command-line arguments. If the first argument is "weather", weather information is fetched and displayed.
    /// Otherwise, a help message is shown.
    /// </param>
    static void Main(string[] args)
    {
      Console.WriteLine("# AskBot here, welcome!");
      IpApi ip = IpApiClient.FetchIp();
      IpApiClient.PrintIp(ip);
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

