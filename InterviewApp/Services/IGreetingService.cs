namespace InterviewApp.Services;

public interface IGreetingService
{
    /// <summary>
    /// Builds and returns the full greeting string combining the time-based greeting,
    /// the language-specific greeting word, and the configured message.
    /// Returns <see cref="string.Empty"/> if configuration validation fails.
    /// </summary>
    string GetGreeting();
}
