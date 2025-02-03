using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Options;
using OnlineChaRobot.Configurations;
using System.Diagnostics;

namespace OnlineChaRobot.Services.DeepSeek
{
    public class DeepSeekService(IOptions<OllamaSettings> options, HttpClient httpClient) : IDeepSeekService
    {
        private readonly Kernel kernel = InitializeKernel(options.Value);
        private static readonly Dictionary<string, List<string>> _chatHistory = new();
        private readonly HttpClient httpClient = httpClient;

        private static Kernel InitializeKernel(OllamaSettings settings)
        {
            try
            {
                Console.WriteLine($"Connecting to Ollama at {settings.BaseUrl} with model {settings.DefaultModel}...");

                var kernel = Kernel.CreateBuilder()
                    .AddOllamaChatCompletion(settings.DefaultModel, settings.BaseUrl)
                    .Build();

                Console.WriteLine("Kernel successfully initialized!");
                return kernel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Ollama: {ex.Message}");
                throw new InvalidOperationException("Failed to connect to the Ollama service. Please make sure Ollama is running.");
            }
        }


        public async Task<string> GetResponseAsync(string prompt, string? model = "deepseek-r1:1.5b")
        {
            try
            {
                model ??= "deepseek-r1:1.5b"; // Default model
                Console.WriteLine($"Sending request to DeepSeek: {prompt} using model {model}");

                // Log chat history check
                if (!_chatHistory.ContainsKey(model))
                {
                    _chatHistory[model] = new();
                }

                // Store chat history
                if (!_chatHistory.ContainsKey(model))
                {
                    _chatHistory[model] = new();
                }
                _chatHistory[model].Add($"User Prompt is: '{prompt}'");

                // Check if the kernel is null
                if (kernel == null)
                {
                    throw new InvalidOperationException("Kernel is not initialized!");
                }


                // Process the model
                var response = await kernel.InvokePromptAsync(prompt);
                Console.WriteLine($"Response received: {response}");

                _chatHistory[model].Add($"AI: {response}");

                return response.ToString();
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException("The request was canceled.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
                throw new InvalidOperationException($"Issue connecting to Ollama: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General DeepSeek Error: {ex.Message}");
                Debug.WriteLine($"Error processing DeepSeek model: {ex.Message}");
                throw new InvalidOperationException("An error occurred while processing the request with DeepSeek. Please try again later.");
            }
        }

        public List<string> GetChatHistory(string model = "deepseek-r1:1.5b") =>
            _chatHistory.ContainsKey(model) ? _chatHistory[model] : new List<string>();
    }
}
