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
    public class WebSocketPublisher : IDisposable, IDomainEventHandler<SensorDataEvent>
    {
        public IWebSocketConnection WebSocketConnection { get; set; }

        public WebSocketPublisher(IWebSocketConnection webSocketConnection)
        {
            WebSocketConnection = webSocketConnection;
            WebSocketConnection.OnMessage = (message) =>
            {
                var configurationEvent = JsonConvert.DeserializeObject<ConfigurationEvent>(message);
                DomainEvents.Raise(configurationEvent);
            };
            WebSocketConnection.OnClose = () => DomainEvents.Deregister(this);
            DomainEvents.Register(this);

        }

        public void Handle(SensorDataEvent sensorDataEvent)
        {
            if (WebSocketConnection.IsAvailable)
            {
                WebSocketConnection.Send(JsonConvert.SerializeObject(sensorDataEvent));
            }
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
