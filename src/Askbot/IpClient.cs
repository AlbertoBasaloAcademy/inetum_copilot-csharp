using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AskBot
{
  public class IpApiClient
  {
    private static readonly HttpClient _httpClient = new HttpClient();

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      WriteIndented = true
    };

    public static IpApi FetchIp()
    {
      try
      {
        HttpResponseMessage response = _httpClient.GetAsync("http://ip-api.com/json/").Result;
        string statusCode = response.StatusCode.ToString();
        if (!response.IsSuccessStatusCode)
        {
          throw new InvalidOperationException($"Error HTTP: {statusCode}");
        }
        string json = response.Content.ReadAsStringAsync().Result;
        IpApi ipApi = JsonSerializer.Deserialize<IpApi>(json, _jsonOptions);
        Console.WriteLine($"## Your IP address is: {ipApi?.Query}");
        string location = ipApi?.City + ", " + ipApi?.RegionName + ", " + ipApi?.Country;
        Console.WriteLine($"- Location: {location}");
        string coordinates = "Lat " + ipApi?.Lat + ", Long " + ipApi?.Lon;
        Console.WriteLine($"- Coordinates: {coordinates}");
        return ipApi;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"> Exception occurred: {ex.Message}");
        return null;
      }
    }
  }
}
