using censudex_auth_service.src.models;
using Grpc.Net.Client;

namespace censudex_auth_service.src.services
{
    /// <summary>
    /// Servicio para gestionar clientes y verificar credenciales utilizando gRPC.
    /// </summary>
    public class ClientsService
    {
        private readonly UserProto.UserService.UserServiceClient _client;

        public ClientsService(string address)
        {
            // Dirección del servicio remoto (puede venir de appsettings.json)
            address = address ?? throw new ArgumentNullException(nameof(address));
            // Crear el canal gRPC
            var channel = GrpcChannel.ForAddress(address);
            // Instanciar el cliente generado automáticamente
            _client = new UserProto.UserService.UserServiceClient(channel);
        }
        public async Task<Auth?> VerifyClientAsync(string username, string password)
        {
            var request = new UserProto.VerifyCredentialsRequest
            {
                Username = username,
                Password = password
            };
            var responseUser = await _client.VerifyCredentialsAsync(request);
            if (string.IsNullOrEmpty(responseUser.Id))
            {
                return null;
            }
            var response = new Auth
            {
                Id = Guid.Parse(responseUser.Id),
                Roles = responseUser.Roles.ToList()
            };
            return response;
        }
        /*
        /// <summary>
        /// Cliente HTTP para realizar solicitudes a la API de usuarios.
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// Constructor que inicializa el servicio con un cliente HTTP y una dirección base.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="baseAddress"></param>
        public ClientsService(HttpClient httpClient, string baseAddress)
        {
            _httpClient = httpClient;
            // Agrega la dirección base la ruta específica para la API de usuarios
            _httpClient.BaseAddress = new Uri(baseAddress + "/api/user/");
        }
        /// <summary>
        /// Verifica las credenciales del cliente enviando una solicitud POST a la API de usuarios.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Objeto Auth</returns>
        public async Task<Auth?> VerifyClientAsync<T>(string username, string password)
        {
            try
            {
                // Envía una solicitud POST con las credenciales del cliente
                var response = await _httpClient.PostAsJsonAsync("verifyCredentials", new
                {
                    Username = username,
                    Password = password
                });
                // Verifica si la respuesta fue exitosa
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed verifying client: {response.StatusCode}");
                    return null;
                }
                // Lee y deserializa la respuesta JSON en un objeto Auth
                response.EnsureSuccessStatusCode();
                // Lee el contenido de la respuesta como una cadena JSON
                var json = await response.Content.ReadAsStringAsync();
                // Deserializa la cadena JSON en un objeto Auth
                var authClient = JsonSerializer.Deserialize<Auth>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                // Devuelve el objeto Auth deserializado
                return authClient;
            }
            // Maneja cualquier excepción que ocurra durante el proceso
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying client: {ex.Message}");
                Auth result = null!;
                return result!;
            }
        }
        */
    }
}