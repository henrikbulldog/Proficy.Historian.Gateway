using Fleck;
using Newtonsoft.Json;
using Proficy.Historian.Gateway.Interfaces;
using Proficy.Historian.Gateway.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.WebSocket
{
    public class WebSocketService : IService
    {
        private readonly WebSocketServer _server;
        private List<WebSocketPublisher> allSockets = new List<WebSocketPublisher>();

        public WebSocketService(WebSocketServiceConfiguration config)
        {
            Log.Information($"Creating server at {config.Address}");
            _server = new WebSocketServer(config.Address);
        }

        public IService Start()
        {
            Log.Information("Web socket server starting");
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Log.Information($"Open web socket to {socket.ConnectionInfo.Origin} on {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                    var publisher = new WebSocketPublisher(socket);
                    allSockets.Add(publisher);
                };
                socket.OnClose = () =>
                {
                    Log.Information($"Close web socket to {socket.ConnectionInfo.Origin} on {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                    var removeme = allSockets.FirstOrDefault(s => s.WebSocketConnection == socket);
                    if(removeme != null)
                    {
                        removeme.Dispose();
                        allSockets.Remove(removeme);
                    }
                };
            });
            return this;
        }

        public IService Stop()
        {
            Log.Information("Web socket server closing.");
            _server.Dispose();
            return this;
        }
    }
}
