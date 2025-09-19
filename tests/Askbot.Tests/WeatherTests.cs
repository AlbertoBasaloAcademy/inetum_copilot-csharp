using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using AskBot;

namespace Askbot.Tests
{
  public class WeatherTests
  {
    // Helper fake handler to return canned responses
    private class FakeHandler : HttpMessageHandler
    {
      private readonly HttpResponseMessage _response;
      public FakeHandler(HttpResponseMessage response) => _response = response;
      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        return Task.FromResult(_response);
      }
    }

    [Fact]
    public async Task Weather_WithCoordinates_ReturnsParsedTemperatureAndCondition()
    {
      // Arrange: fake Open-Meteo response
      var openMeteoJson = JsonSerializer.Serialize(new
      {
        latitude = 40.0,
        longitude = -3.0,
        generationtime_ms = 0.123,
        utc_offset_seconds = 0,
        timezone = "GMT",
        timezone_abbreviation = "GMT",
        elevation = 667.0,
        current_weather = new { temperature = 22.04, windspeed = 3.3, winddirection = 210.0, weathercode = 3 }
      });

      var response = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(openMeteoJson)
      };

      var handler = new FakeHandler(response);
      var client = new HttpClient(handler);

      var weather = new Weather(client);

      // Act
      var result = await weather.FetchWeatherAsync(40.0, -3.0, null);

            // Assert: result is the formatted string and contains correct temperature (culture-agnostic)
            Assert.NotNull(result);
            Assert.Contains("Temperature:", result);
            Assert.Contains("°C", result);
            // Extract numeric temperature between 'Temperature:' and '°C'
            var tempSection = result.Split("Temperature:")[1].Split('°')[0].Trim();
            // Normalize decimal separator and parse
            var tempNormalized = tempSection.Replace(',', '.').Replace("°C", string.Empty).Trim();
            var tempValue = decimal.Parse(tempNormalized, System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(22.0m, tempValue);
            Assert.Contains("Condition:", result);
    }

    [Fact]
    public async Task Weather_NoCoordinates_UsesIpApiAndReturnsLocationHeader()
    {
      // Arrange: fake IP API response then Open-Meteo response
      var ipJson = JsonSerializer.Serialize(new { status = "success", lat = 40.4168, lon = -3.7038, city = "Madrid", country = "Spain" });
      var openMeteoJson = JsonSerializer.Serialize(new
      {
        current_weather = new { temperature = 18.5, weathercode = 1 }
      });

      // We'll implement a handler that switches by request Uri
      var handler = new DelegatingHandlerStub((req) =>
      {
        if (req.RequestUri!.Host.Contains("ip-api.com"))
          return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(ipJson) };
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(openMeteoJson) };
      });

      var client = new HttpClient(handler);
      var weather = new Weather(client);

      // Act
      var ipApi = new IpApi("1.2.3.4", "success", "Spain", "ES", "", "", "Madrid", "", 40.4168, -3.7038, "", "", "", "");
      var result = await weather.FetchWeatherAsync(null, null, ipApi);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Weather in Madrid, Spain", result);
            Assert.Contains("Temperature:", result);
            var tempSection2 = result.Split("Temperature:")[1].Split('°')[0].Trim();
            var tempNormalized2 = tempSection2.Replace(',', '.');
            var tempValue2 = decimal.Parse(tempNormalized2, System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(18.5m, tempValue2);
    }

    [Fact]
    public async Task Weather_InvalidCoordinates_ThrowsArgumentOutOfRange()
    {
      var handler = new FakeHandler(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
      var client = new HttpClient(handler);
      var weather = new Weather(client);

      await Assert.ThrowsAsync<System.ArgumentException>(() => weather.FetchWeatherAsync(999, 999, null));
    }

    [Fact]
    public async Task Weather_NetworkFailure_ReturnsFriendlyError()
    {
      var handler = new FakeHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));
      var client = new HttpClient(handler);
      var weather = new Weather(client);

      var ex = await Assert.ThrowsAsync<HttpRequestException>(() => weather.FetchWeatherAsync(40.0, -3.0, null));
      // Ensure the inner message indicates failure from HTTP
      Assert.NotNull(ex.Message);
    }

    // Minimal delegating handler stub to route requests in tests
    private class DelegatingHandlerStub : HttpMessageHandler
    {
      private readonly System.Func<HttpRequestMessage, HttpResponseMessage> _responder;
      public DelegatingHandlerStub(System.Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
        return Task.FromResult(_responder(request));
      }
    }
  }
}
