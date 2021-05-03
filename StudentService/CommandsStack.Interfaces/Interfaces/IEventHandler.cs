using Common.Interfaces;

namespace Commands.Infrastructure.Interfaces
{
    public interface IEventHandler<T> where T : IEvent
    {
        void Handle(T @event);
    }
}
