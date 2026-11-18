using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InterviewApp.Models;

namespace InterviewApp.Services;

public class GreetingService(
    IOptions<GreetingOptions> options,
    ILogger<GreetingService> logger,
    ITimeGreetingService timeGreetingService) : IGreetingService
{
    private readonly GreetingOptions _options = options.Value;

    private static readonly System.Collections.Generic.Dictionary<string, string> LanguageGreetings =
        new(System.StringComparer.OrdinalIgnoreCase)
        {
            { "English",   "Hello" },
            { "Afrikaans", "Hallo" },
            { "Zulu",      "Sawubona" }
        };

    public string GetGreeting()
    {
        logger.LogInformation("GreetingService starting.");

        if (string.IsNullOrWhiteSpace(_options.Message))
        {
            logger.LogError("Configuration error: 'Message' is null or empty.");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(_options.Language))
        {
            logger.LogError("Configuration error: 'Language' is null or empty.");
            return string.Empty;
        }

        string languageGreeting;
        if (!LanguageGreetings.TryGetValue(_options.Language, out languageGreeting!))
        {
            logger.LogWarning("Unsupported language '{Language}'. Falling back to English.", _options.Language);
            languageGreeting = LanguageGreetings["English"];
        }

        string timeGreeting = timeGreetingService.GetGreeting();
        string fullGreeting = $"{timeGreeting} - {languageGreeting} - {_options.Message}";

        logger.LogInformation("Displaying message: {Message}", fullGreeting);

        return fullGreeting;
    }

    public void Run()
    {
        string greeting = GetGreeting();
        if (!string.IsNullOrEmpty(greeting))
            Console.WriteLine(greeting);
    }
}
