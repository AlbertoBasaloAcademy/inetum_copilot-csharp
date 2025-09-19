
namespace AskBot
{
  class Program
  {
    /// <summary>
    /// Entry point of the AskBot application.
    /// Supports: `weather` and `weather <lat> <lon>`
    /// </summary>
    static async System.Threading.Tasks.Task Main(string[] args)
    {
      Console.WriteLine("# AskBot here, welcome!");
      IpApi ip = IpApiClient.FetchIp();
      IpApiClient.PrintIp(ip);

      if (args.Length > 0)
      {
        var cmd = args[0].ToLower();
        if (cmd == "weather")
        {
          // Usage: askbot weather OR askbot weather <lat> <lon>
          double? lat = null;
          double? lon = null;
          try
          {
            if (args.Length == 3)
            {
              lat = double.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture);
              lon = double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
            }

            Console.WriteLine("## Fetching weather information...");
            var weather = new Weather();
            string weatherInfo = await weather.FetchWeatherAsync(lat, lon, ip);
            Console.WriteLine(weatherInfo);
          }
          catch (ArgumentException aex)
          {
            Console.WriteLine($"> {aex.Message}");
            Environment.ExitCode = 1;
          }
          catch (System.Net.Http.HttpRequestException)
          {
            Console.WriteLine("> Weather service unavailable — try again later");
            Environment.ExitCode = 2;
          }
          catch (System.Exception ex)
          {
            Console.WriteLine($"> Unexpected error: {ex.Message}");
            Environment.ExitCode = 3;
          }
        }
        else if (cmd == "money")
        {
          try
          {
            Console.WriteLine("## Fetching currency and exchange rates...");
            var report = await Askbot.Money.GetMoneyReportAsync(ip);
            Console.WriteLine(report);
          }
          catch (System.Net.Http.HttpRequestException)
          {
            Console.WriteLine("> Currency/rates service unavailable — try again later");
            Environment.ExitCode = 2;
          }
          catch (System.Exception ex)
          {
            Console.WriteLine($"> Unexpected error: {ex.Message}");
            Environment.ExitCode = 3;
          }
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
    }

    private static void PrintHelpMessage()
    {
  Console.WriteLine("## Available commands:");
  Console.WriteLine("  - `weather` :  Fetch the current weather information for your IP location");
  Console.WriteLine("  - `weather <lat> <lon>` : Fetch weather for specific coordinates (decimal degrees)");
  Console.WriteLine("  - `money` :  Show your country's official currency and exchange rates vs EUR, USD, GBP, CHF");
    }
  }
}

