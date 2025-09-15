using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AskBot
{
    /// <summary>
    /// Cliente para consumir la API pública de IP y obtener información de geolocalización.
    /// </summary>
    public class IpApiClient
    {
        /// <summary>
        /// Instancia estática de <see cref="HttpClient"/> utilizada para realizar solicitudes HTTP.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Opciones de serialización JSON para deserializar la respuesta de la API.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// Imprime un reporte con la información de la IP obtenida.
        /// </summary>
        /// <param name="ipApi">Instancia de <see cref="IpApi"/> con los datos de la IP y ubicación.</param>
        public static void PrintIp(IpApi ipApi)
        {
            if (ipApi == null)
            {
                Console.WriteLine("> No se pudo obtener información de la IP.");
                return;
            }
            Console.WriteLine($"## Your IP address is: {ipApi.Query}");
            string location = $"{ipApi.City}, {ipApi.RegionName}, {ipApi.Country}";
            Console.WriteLine($"- Location: {location}");
            string coordinates = $"Lat {ipApi.Lat}, Long {ipApi.Lon}";
            Console.WriteLine($"- Coordinates: {coordinates}");
        }

        /// <summary>
        /// Realiza una solicitud a la API de IP y obtiene información de la dirección IP pública.
        /// </summary>
        /// <returns>
        /// Una instancia de <see cref="IpApi"/> con los datos de la IP y ubicación, o <c>null</c> si ocurre una excepción.
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
