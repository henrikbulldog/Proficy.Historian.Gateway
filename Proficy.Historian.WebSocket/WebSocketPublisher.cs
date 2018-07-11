using Fleck;
using Newtonsoft.Json;
using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.WebSocket
{
    public class WebSocketPublisher : IPublisher, IDisposable
    {
        public IWebSocketConnection WebSocketConnection { get; set; }

        public IHistorian Historian { get; set; }

        public Action<string> OnMessage
        {
            get
            {
                if(WebSocketConnection != null)
                {
                    return WebSocketConnection.OnMessage;
                }
                return null;
            }
            set
            {
                if (WebSocketConnection != null)
                {
                    WebSocketConnection.OnMessage = value;
                }
            }
        }

        public WebSocketPublisher(IWebSocketConnection webSocketConnection)
        {
            WebSocketConnection = webSocketConnection;
        }

        public void SendMessage(object message)
        {
            WebSocketConnection.Send(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            Historian.Stop();
        }
    }
}
