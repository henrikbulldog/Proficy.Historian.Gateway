using Fleck;
using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.WebSocket
{
    public class WebSocketPublisher : IDisposable, IDomainEventHandler
    {
        public IWebSocketConnection WebSocketConnection { get; set; }

        public WebSocketPublisher(IWebSocketConnection webSocketConnection)
        {
            WebSocketConnection = webSocketConnection;
            WebSocketConnection.OnMessage = (message) =>
            {
                var e = JsonConvert.DeserializeObject<ConfigurationEvent>(message);
                DomainEvents.Raise(e);
            };
            DomainEvents.Register<SensorDataEvent>(this);
        }

        public void Handle(IDomainEvent domainEvent)
        {
            WebSocketConnection.Send(JsonConvert.SerializeObject(domainEvent));
        }

        public void Dispose()
        {
            if (WebSocketConnection != null)
            {
                WebSocketConnection.Close();
            }
        }
    }
}
