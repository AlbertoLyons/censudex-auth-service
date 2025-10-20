using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using censudex_auth_service.src.models;

namespace censudex_auth_service.src.services
{
    public class ClientsService
    {
        private readonly HttpClient _httpClient;
        public ClientsService(HttpClient httpClient, string baseAddress)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseAddress + "/api/user/");
        }
        public async Task<Auth?> VerifyClientAsync<T>(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("verifyCredentials", new
                {
                    Username = username,
                    Password = password
                });
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed verifying client: {response.StatusCode}");
                    return null;
                }
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var authClient = JsonSerializer.Deserialize<Auth>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Console.WriteLine($"Verified client: {authClient?.UserName}");
                return authClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying client: {ex.Message}");
                Auth result = null!;
                return result!;
            }
        }
    }
}