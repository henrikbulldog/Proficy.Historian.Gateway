using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.DomainEvent
{
    public static class DomainEvents
    {
        private static Dictionary<Type, List<IDomainEventHandler>> handlers = new Dictionary<Type, List<IDomainEventHandler>>();

        public static void Register<T>(IDomainEventHandler eventHandler)
            where T : IDomainEvent
        {
            if(!handlers.ContainsKey(typeof(T)))
            {
                handlers.Add(typeof(T), new List<IDomainEventHandler>());
            }
            handlers[typeof(T)].Add(eventHandler);
        }

        public static void Raise<T>(T domainEvent)
            where T : IDomainEvent
        {
            foreach (var handler in handlers[domainEvent.GetType()])
            {
                handler.Handle(domainEvent);
            }
        }
    }
}
