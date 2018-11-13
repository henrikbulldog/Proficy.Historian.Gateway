using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.DomainEvent
{
    public static class DomainEvents
    {
        private static Dictionary<Type, List<object>> handlers = new Dictionary<Type, List<object>>();

        public static void Register<T>(IDomainEventHandler<T> eventHandler)
            where T : IDomainEvent
        {
            lock (handlers)
            {
                if (!handlers.ContainsKey(typeof(T)))
                {
                    handlers.Add(typeof(T), new List<object>());
                }
                handlers[typeof(T)].Add(eventHandler);
            }
        }

        public static void Deregister<T>(IDomainEventHandler<T> eventHandler)
            where T : IDomainEvent
        {
            lock (handlers)
            {
                if (handlers.ContainsKey(typeof(T)) && handlers[typeof(T)].Contains(eventHandler))
                {
                    handlers[typeof(T)].Remove(eventHandler);
                }
            }
        }

        public static void Raise<T>(T domainEvent)
            where T : IDomainEvent
        {
            lock (handlers)
            {
                if (handlers.ContainsKey(typeof(T)))
                {
                    foreach (IDomainEventHandler<T> handler in handlers[typeof(T)])
                    {
                        Task.Run(() => handler.Handle(domainEvent));
                    }
                }
            }
        }
    }
}
