using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using InterviewApp.Models;
using InterviewApp.Services;

namespace InterviewApp.Tests;

public class GreetingServiceTests
{
    private static GreetingService BuildService(
        string message,
        string language,
        Mock<ILogger<GreetingService>>? loggerMock = null,
        Mock<ITimeGreetingService>? timeMock = null)
    {
        var options = Options.Create(new GreetingOptions { Message = message, Language = language });
        var logger = (loggerMock ?? new Mock<ILogger<GreetingService>>()).Object;
        if (timeMock == null)
        {
            timeMock = new Mock<ITimeGreetingService>();
            timeMock.Setup(t => t.GetGreeting()).Returns("Good morning");
        }
        return new GreetingService(options, logger, timeMock.Object);
    }

    // Language greeting word tests

    [Fact]
    public void GetGreeting_English_ContainsHello()
    {
        var svc = BuildService("Welcome!", "English");
        Assert.Contains("Hello", svc.GetGreeting());
    }

    [Fact]
    public void GetGreeting_Afrikaans_ContainsHallo()
    {
        var svc = BuildService("Welcome!", "Afrikaans");
        Assert.Contains("Hallo", svc.GetGreeting());
    }

    [Fact]
    public void GetGreeting_Zulu_ContainsSawubona()
    {
        var svc = BuildService("Welcome!", "Zulu");
        Assert.Contains("Sawubona", svc.GetGreeting());
    }

    // Unsupported language falls back and logs a warning

    [Fact]
    public void GetGreeting_UnsupportedLanguage_FallsBackToEnglish()
    {
        var svc = BuildService("Welcome!", "Klingon");
        string result = svc.GetGreeting();
        Assert.Contains("Hello", result);
    }

    [Fact]
    public void GetGreeting_UnsupportedLanguage_LogsWarning()
    {
        var loggerMock = new Mock<ILogger<GreetingService>>();
        var svc = BuildService("Welcome!", "Klingon", loggerMock);
        svc.GetGreeting();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Unsupported language") || v.ToString()!.Contains("Klingon")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // Validation: empty Message causes early exit

    [Fact]
    public void GetGreeting_EmptyMessage_ReturnsEmptyString()
    {
        var svc = BuildService(string.Empty, "English");
        Assert.Equal(string.Empty, svc.GetGreeting());
    }

    [Fact]
    public void GetGreeting_EmptyMessage_LogsError()
    {
        var loggerMock = new Mock<ILogger<GreetingService>>();
        var svc = BuildService(string.Empty, "English", loggerMock);
        svc.GetGreeting();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // Validation: empty Language causes early exit

    [Fact]
    public void GetGreeting_EmptyLanguage_ReturnsEmptyString()
    {
        var svc = BuildService("Welcome!", string.Empty);
        Assert.Equal(string.Empty, svc.GetGreeting());
    }

    [Fact]
    public void GetGreeting_EmptyLanguage_LogsError()
    {
        var loggerMock = new Mock<ILogger<GreetingService>>();
        var svc = BuildService("Welcome!", string.Empty, loggerMock);
        svc.GetGreeting();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Language")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // Time-based greeting is prepended

    [Theory]
    [InlineData("Good morning")]
    [InlineData("Good afternoon")]
    [InlineData("Good evening")]
    public void GetGreeting_PrependTimeGreeting(string timeGreeting)
    {
        var timeMock = new Mock<ITimeGreetingService>();
        timeMock.Setup(t => t.GetGreeting()).Returns(timeGreeting);
        var svc = BuildService("Welcome!", "English", timeMock: timeMock);

        string result = svc.GetGreeting();

        Assert.StartsWith(timeGreeting, result);
    }

    [Fact]
    public void GetGreeting_FullFormat_IsCorrect()
    {
        var timeMock = new Mock<ITimeGreetingService>();
        timeMock.Setup(t => t.GetGreeting()).Returns("Good morning");
        var svc = BuildService("Welcome!", "Zulu", timeMock: timeMock);

        string result = svc.GetGreeting();

        Assert.Equal("Good morning - Sawubona - Welcome!", result);
    }
}
