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
    public class WebSocketService : IPublisher
    {
        private readonly WebSocketServer _server;
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();

        public WebSocketService(WebSocketServiceConfiguration config)
        {
            Console.WriteLine($"Creating server at {config.Address}");
            _server = new WebSocketServer(config.Address);
        }

        public IService Start()
        {
            Console.WriteLine("Web socket server starting");
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine($"Open web socket to {socket.ConnectionInfo.Origin} on {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine($"Close web socket to {socket.ConnectionInfo.Origin} on {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message => Console.WriteLine(message);
            });
            return this;
        }

        public IService Stop()
        {
            Console.WriteLine("HTTP server closing.");
            _server.Dispose();
            return this;
        }

        public void SendMessage(object message)
        {
            foreach (var socket in allSockets)
            {
                socket.Send(JsonConvert.SerializeObject(message));
            }
        }
    }
}
