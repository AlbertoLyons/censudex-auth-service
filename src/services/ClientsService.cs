using censudex_auth_service.src.models;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http;

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
            var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
            // Instanciar el cliente generado automáticamente
            _client = new UserProto.UserService.UserServiceClient(channel);
        }
        /// <summary>
        /// Verifica las credenciales del cliente llamando al servicio gRPC.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Auth model</returns>
        public async Task<Auth?> VerifyClientAsync(string username, string password)
        {
            // Realizar la llamada gRPC para verificar las credenciales
            try
            {
                // Se crea la solicitud gRPC
                var request = new UserProto.VerifyCredentialsRequest
                {
                    Username = username,
                    Password = password
                };
                // Se realiza la llamada al servicio gRPC
                var responseUser = await _client.VerifyCredentialsAsync(request);
                // Si no se encuentra el usuario, devolver null
                if (string.IsNullOrEmpty(responseUser.Id))
                {
                    return null;
                }
                // Mapear la respuesta gRPC al modelo Auth
                var response = new Auth
                {
                    Id = Guid.Parse(responseUser.Id),
                    Roles = responseUser.Roles.ToList()
                };
                // Devolver el modelo Auth
                return response;
            }
            // En caso de error, devolver null
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying client: {ex.Message}");
                Auth result = null!;
                return result!;
            }
        }
    }
}