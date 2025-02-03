namespace OnlineChaRobot.Configurations
{
    public class OllamaSettings
    {
        public required string BaseUrl { get; set; } = "http://localhost:11434";
        public required string DefaultModel { get; set; } = "deepseek-r1:1.5b";
        public double Temperature { get; set; } = 0.7; // میزان خلاقیت مدل (پیش‌فرض: 0.7)
    }
}
