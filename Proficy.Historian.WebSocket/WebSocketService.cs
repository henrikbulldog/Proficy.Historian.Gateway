using Fleck;
using Proficy.Historian.Gateway.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proficy.Historian.WebSocket
{
    public class WebSocketService : IService
    {
        private readonly WebSocketServer server;
        private List<WebSocketPublisher> allSockets = new List<WebSocketPublisher>();

        public WebSocketService(WebSocketServiceConfiguration config)
        {
            Log.Information($"Creating server at {config.Address}");
            server = new WebSocketServer(config.Address);
        }

        public bool Start()
        {
            try
            {
                server.Start(socket =>
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
                        if (removeme != null)
                        {
                            removeme.Dispose();
                            allSockets.Remove(removeme);
                        }
                    };
                });
            }
            catch (Exception exc)
            {
                Log.Error($"Web socker server: Error while initializing. {exc}");
                return false;
            }

            Log.Information("Web socket server startet");
            return true;
        }

        public bool Stop()
        {
            server.Dispose();
            Log.Information("Web socket server closed");
            return true;
        }
    }
}
