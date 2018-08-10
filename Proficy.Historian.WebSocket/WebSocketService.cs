using Fleck;
using Newtonsoft.Json;
using Proficy.Historian.Gateway.Shared;
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
        private Action<IPublisher> _onOpen;

        public WebSocketService(WebSocketServiceConfiguration config, Action<IPublisher> onOpen)
        {
            _onOpen = onOpen;
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
                    if (_onOpen != null)
                    {
                        _onOpen(publisher);
                    }
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
