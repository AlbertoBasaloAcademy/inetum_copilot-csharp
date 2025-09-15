using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AskBot
{
    /// <summary>
    /// Cliente para consumir la API p�blica de IP y obtener informaci�n de geolocalizaci�n.
    /// </summary>
    public class IpApiClient
    {
        /// <summary>
        /// Instancia est�tica de <see cref="HttpClient"/> utilizada para realizar solicitudes HTTP.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Opciones de serializaci�n JSON para deserializar la respuesta de la API.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// Imprime un reporte con la informaci�n de la IP obtenida.
        /// </summary>
        /// <param name="ipApi">Instancia de <see cref="IpApi"/> con los datos de la IP y ubicaci�n.</param>
        public static void PrintIp(IpApi ipApi)
        {
            if (ipApi == null)
            {
                Console.WriteLine("> No se pudo obtener informaci�n de la IP.");
                return;
            }
            Console.WriteLine($"## Your IP address is: {ipApi.Query}");
            string location = $"{ipApi.City}, {ipApi.RegionName}, {ipApi.Country}";
            Console.WriteLine($"- Location: {location}");
            string coordinates = $"Lat {ipApi.Lat}, Long {ipApi.Lon}";
            Console.WriteLine($"- Coordinates: {coordinates}");
        }

        /// <summary>
        /// Realiza una solicitud a la API de IP y obtiene informaci�n de la direcci�n IP p�blica.
        /// </summary>
        /// <returns>
        /// Una instancia de <see cref="IpApi"/> con los datos de la IP y ubicaci�n, o <c>null</c> si ocurre una excepci�n.
        /// </returns>
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
