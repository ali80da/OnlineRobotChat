namespace OnlineChaRobot.Services.Ollama
{
    public interface IOllamaClient
    {


        Task<string> SendPromptAsync(string prompt, string model, CancellationToken cancellationToken);

    }
}
