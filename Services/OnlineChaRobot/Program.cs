using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using OnlineChaRobot.Configurations;
using OnlineChaRobot.Services.DeepSeek;
using OnlineChaRobot.Services.Ollama;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.DarkBlue;

    // Add services to the container.
    // Register HttpClient FIRST before using AddOllamaChatCompletion
    builder.Services.AddHttpClient();

    // افزودن تنظیمات Ollama
    builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("Ollama"));

    // ثبت HttpClient برای ارتباط با Ollama
    //builder.Services.AddHttpClient<IOllamaClient, OllamaClient>();
    // Initialize Semantic Kernel with Ollama
    builder.Services.AddSingleton(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;

        return Kernel.CreateBuilder()
            .AddOllamaChatCompletion(settings.DefaultModel, settings.BaseUrl)
            .Build();
    });

    // ثبت سرویس هوش مصنوعی
    builder.Services.AddSingleton<IDeepSeekService, DeepSeekService>();


    builder.Services.AddControllers();

    // افزودن Swagger و OpenAPI
    // Configuring OpenAPI
    //builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ChatBot API",
            Version = "v1",
            Description = "API to communicate with the DeepSeek model via Ollama"
        });
    });


    // 📌 اجرای Ollama از مسیر محلی
    //var ollamaPath = Path.Combine(AppContext.BaseDirectory, "Tools", "Ollama", "ollama.exe");
    //var ollamaModelsPath = Path.Combine(AppContext.BaseDirectory, "Tools", "Ollama", "Models");

    //if (File.Exists(ollamaPath))
    //{
    //    Console.WriteLine("🚀 در حال اجرای Ollama...");
    //    var process = new Process
    //    {
    //        StartInfo = new ProcessStartInfo
    //        {
    //            FileName = ollamaPath,
    //            Arguments = $"serve --dir \"{ollamaModelsPath}\"",
    //            RedirectStandardOutput = true,
    //            RedirectStandardError = true,
    //            UseShellExecute = false,
    //            CreateNoWindow = true
    //        }
    //    };
    //    process.Start();
    //}


}
var app = builder.Build();
{

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("Environment: IsDevelopment");
        //app.MapOpenApi();
    }
    else if (app.Environment.IsStaging())
    {
        Console.WriteLine("Environment: IsStaging");
    }
    else if (app.Environment.IsProduction())
    {
        Console.WriteLine("Environment: IsProduction");
        // JUST 4


        app.UseHsts();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatRobot API v1");
        options.RoutePrefix = string.Empty; // دسترسی به Swagger در مسیر اصلی
    });


    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

}
app.Run();
