using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InterviewApp.MediatR.Commands;
using InterviewApp.Services;

namespace InterviewApp.MediatR.Handlers;

public class GreetUserHandler(IGreetingService greetingService) : IRequestHandler<GreetUserCommand, string>
{
    public Task<string> Handle(GreetUserCommand request, CancellationToken cancellationToken)
    {
        string greeting = greetingService.GetGreeting();
        return Task.FromResult(greeting);
    }
}
