using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AskBot
{
  public class Weather
  {
    private static readonly HttpClient _http = new HttpClient();

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Fetches current weather. If lat/lon are null, attempts to use the provided ipApi coordinates.
    /// </summary>
    /// <param name="lat">Latitude or null to use IP location.</param>
    /// <param name="lon">Longitude or null to use IP location.</param>
    /// <param name="ipApi">IP info used when coordinates are not provided. May be null.</param>
    public async Task<string> FetchWeatherAsync(double? lat, double? lon, IpApi ipApi)
    {
      // Resolve coordinates
      double useLat, useLon;
      if (lat.HasValue && lon.HasValue)
      {
        if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
          throw new ArgumentException("Invalid coordinates. Latitude must be between -90 and 90 and longitude between -180 and 180.");
        useLat = lat.Value;
        useLon = lon.Value;
      }
      else
      {
        if (ipApi == null)
          throw new ArgumentException("Unable to resolve location");
        useLat = ipApi.Lat;
        useLon = ipApi.Lon;
      }

      // Call Open-Meteo current weather API
      string url = $"https://api.open-meteo.com/v1/forecast?latitude={useLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={useLon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&current_weather=true&timezone=UTC&hourly=precipitation_probability";

      using var resp = await _http.GetAsync(url).ConfigureAwait(false);
      resp.EnsureSuccessStatusCode();
      string json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

      using var doc = JsonDocument.Parse(json);
      var root = doc.RootElement;

      // Extract current_weather
      if (!root.TryGetProperty("current_weather", out var current))
        throw new Exception("Weather response missing current_weather");

      double temperature = current.GetProperty("temperature").GetDouble();
      int weatherCode = current.GetProperty("weathercode").GetInt32();

      // Precipitation probability: try to get hourly.precipitation_probability at the current hour index
      string precipitationText = "N/A";
      if (root.TryGetProperty("hourly", out var hourly) && hourly.TryGetProperty("precipitation_probability", out var ppArr) && hourly.TryGetProperty("time", out var timeArr))
      {
        // Find the index of current.time in hourly.time
        if (current.TryGetProperty("time", out var currentTime) && currentTime.ValueKind == JsonValueKind.String)
        {
          string? curTime = currentTime.GetString();
          if (!string.IsNullOrEmpty(curTime))
          {
            int idx = -1;
            for (int i = 0; i < timeArr.GetArrayLength(); i++)
            {
              if (timeArr[i].GetString() == curTime)
              {
                idx = i;
                break;
              }
            }
            if (idx >= 0 && idx < ppArr.GetArrayLength())
            {
              double pp = ppArr[idx].GetDouble();
              precipitationText = Math.Round(pp) + "%";
            }
          }
        }
      }

      string condition = WeatherCodeToText(weatherCode);

      // Header with location when available
      string header = "Weather";
      if (ipApi != null && !string.IsNullOrWhiteSpace(ipApi.City) && !double.IsNaN(ipApi.Lat))
      {
        header = $"Weather in {ipApi.City}, {ipApi.Country}";
      }

      return $"🌤️ {header}:\n\tTemperature: {Math.Round(temperature, 1):0.0}°C\n\tPrecipitation: {precipitationText}\n\tCondition: {condition}";
    }

    private static string WeatherCodeToText(int code)
    {
      // Mapping based on Open-Meteo / WMO weather codes (simplified)
      return code switch
      {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 => "Fog",
        48 => "Depositing rime fog",
        51 => "Light drizzle",
        53 => "Moderate drizzle",
        55 => "Dense drizzle",
        56 => "Light freezing drizzle",
        57 => "Dense freezing drizzle",
        61 => "Slight rain",
        63 => "Moderate rain",
        65 => "Heavy rain",
        66 => "Light freezing rain",
        67 => "Heavy freezing rain",
        71 => "Slight snow fall",
        73 => "Moderate snow fall",
        75 => "Heavy snow fall",
        77 => "Snow grains",
        80 => "Slight rain showers",
        81 => "Moderate rain showers",
        82 => "Violent rain showers",
        85 => "Slight snow showers",
        86 => "Heavy snow showers",
        95 => "Thunderstorm",
        96 => "Thunderstorm with slight hail",
        99 => "Thunderstorm with heavy hail",
        _ => "Unknown"
      };
    }
  }
}