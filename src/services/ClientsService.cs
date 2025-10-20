using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
        public async Task<string> VerifyClientAsync<T>(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("verifyCredentials", new
                {
                    Username = username,
                    Password = password
                });
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<T>();
                var resultString = result!.ToString();
                if (resultString!.Contains("Client"))
                {
                    return "200 OK. Client verified";
                }
                else if (resultString!.Contains("Admin"))
                {
                    return "200 OK. Admin verified";
                }
                else return "Error verifying client";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var result = "";
                if (ex.Message.Contains("400"))
                {
                    result = "Error 400. Invalid credentials";
                }
                else result = "Error verifying client";
                return result;
            }
        }
    }
}