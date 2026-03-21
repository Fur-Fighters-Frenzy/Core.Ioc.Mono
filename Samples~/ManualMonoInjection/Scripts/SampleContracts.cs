using Validosik.Core.Ioc.Interfaces;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public interface IClockService : IContainableService
    {
        string GetTimestamp();
    }

    public interface IGreetingService : IContainableService
    {
        string BuildGreeting(string receiverName);
    }
}
