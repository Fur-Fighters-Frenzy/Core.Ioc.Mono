using System;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SampleClockService : IClockService
    {
        public string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }

    public sealed class SampleGreetingService : IGreetingService
    {
        private readonly IClockService _clockService;

        public SampleGreetingService(IClockService clockService)
        {
            _clockService = clockService;
        }

        public string BuildGreeting(string receiverName)
        {
            return "[" + _clockService.GetTimestamp() + "] Hello, " + receiverName + " from IoC.Mono";
        }
    }
}
