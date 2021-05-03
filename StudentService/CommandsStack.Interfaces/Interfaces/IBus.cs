using Common.Interfaces;

namespace CommandsStack.Infrastructure
{
    public interface IBus
    {
        void Handle<T>(T theEvent) where T : IEvent;        
    }
}
