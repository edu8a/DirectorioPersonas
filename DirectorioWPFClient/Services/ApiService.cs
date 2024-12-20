using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DirectorioWPFClient.Models;

namespace DirectorioWPFClient.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://localhost:7245/api/")
            };

        }
        public async Task<bool> CreatePersonaAsync(Persona nuevaPersona)
        {
            var json = JsonConvert.SerializeObject(nuevaPersona);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Personas", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePersonaAsync(string identificacion)
        {
            var response = await _httpClient.DeleteAsync($"Personas/{identificacion}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<Persona>> GetPersonasAsync()
        {
            var response = await _httpClient.GetAsync("Personas");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Persona>>(content);
        }
      

    }
}
