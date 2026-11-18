using System;

namespace InterviewApp.Services;

public class TimeGreetingService : ITimeGreetingService
{
    public string GetGreeting()
    {
        int hour = DateTime.Now.Hour;

        return hour switch
        {
            >= 0 and <= 11  => "Good morning",
            >= 12 and <= 17 => "Good afternoon",
            _               => "Good evening"
        };
    }
}
