using System;
using System.Collections.Generic;
using System.Linq;
using Commands.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsStack.Infrastructure
{
    public class Bus : IBus
    {           
        private readonly IServiceProvider serviceProvider;

        public Bus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        void IBus.Handle<T>(T @event)
        {
            IEnumerable<IEventHandler<T>> handlers = this.serviceProvider.GetServices<IEventHandler<T>>();

            handlers.ToList().ForEach(handler => ((dynamic)handler).Handle(@event));                   
        }       
    }
}
