namespace OnlineChaRobot.Services.DeepSeek
{
    public interface IDeepSeekService
    {

        //Task<string> GetResponseAsync(string prompt);
        Task<string> GetResponseAsync(string prompt, string? model = "deepseek-r1:1.5b");
        List<string> GetChatHistory(string model = "deepseek-r1:1.5b");

    }
}