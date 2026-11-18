using MediatR;
using Microsoft.Extensions.Options;
using InterviewApp.MediatR.Queries;
using InterviewApp.Models;
using InterviewApp.Services;

namespace InterviewApp.MediatR.Handlers;

/// <summary>
/// Handles <see cref="GetTimeGreetingQuery"/>.
/// Combines the time-based greeting from <see cref="ITimeGreetingService"/>
/// with the configured message from <see cref="GreetingOptions"/> using MediatR,
/// demonstrating decoupled composition (Task 8).
/// </summary>
public class GetTimeGreetingHandler(
    ITimeGreetingService timeGreetingService,
    IOptions<GreetingOptions> options) : IRequestHandler<GetTimeGreetingQuery, string>
{
    private readonly GreetingOptions _options = options.Value;

    public Task<string> Handle(GetTimeGreetingQuery request, CancellationToken cancellationToken)
    {
        string timeGreeting = timeGreetingService.GetGreeting();

        // Combine with the configured message as required by Task 8
        string combined = string.IsNullOrWhiteSpace(_options.Message)
            ? timeGreeting
            : $"{timeGreeting} - {_options.Message}";

        return Task.FromResult(combined);
    }
}
