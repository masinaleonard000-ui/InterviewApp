using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MediatR;
using InterviewApp.Models;
using InterviewApp.Services;
using InterviewApp.MediatR.Commands;
using InterviewApp.MediatR.Queries;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<GreetingOptions>(context.Configuration.GetSection("Greeting"));
                services.AddTransient<IGreetingService, GreetingService>();
                services.AddTransient<ITimeGreetingService, TimeGreetingService>();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            })
            .Build();

        var mediator = host.Services.GetRequiredService<IMediator>();

        // Task 7: Replace the direct GreetingService.Run() call with MediatR Send().
        // GreetUserHandler calls IGreetingService.GetGreeting() which combines:
        // time-based greeting + language greeting word + configured message.
        string greeting = await mediator.Send(new GreetUserCommand());
        if (!string.IsNullOrEmpty(greeting))
            Console.WriteLine(greeting);

        // Task 8: GetTimeGreetingQuery demonstrates a dedicated MediatR query
        // for the time-based greeting, combined with the configured message.
        await mediator.Send(new GetTimeGreetingQuery());
    }
}
