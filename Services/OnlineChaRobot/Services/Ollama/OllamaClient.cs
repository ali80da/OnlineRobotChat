using Microsoft.Extensions.Options;
using OnlineChaRobot.Configurations;
using System.Net.Http;

namespace OnlineChaRobot.Services.Ollama
{
    public class OllamaClient(IOptions<OllamaSettings> options, HttpClient httpClient) : IOllamaClient
    {


        private readonly OllamaSettings _settings = options.Value;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<string> SendPromptAsync(string prompt, string model, CancellationToken cancellationToken)
        {
            var request = new { model, prompt };
            var response = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/api/generate", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return result;
        }


    }
}
