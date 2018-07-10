using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Api
{
    public class SimpleWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection _wsClients = new WebSocketCollection();

        public override void OnOpen()
        {
            _wsClients.Add(this);
            base.OnOpen();
        }

        public override void OnClose()
        {
            _wsClients.Remove(this);
            base.OnClose();
        }

        public void SendMessage(SimpleMessage message)
        {
            if (message.SessionId == Guid.Empty)
            {
                SendBroadcastMessage(message);
            }

            else
            {
                SendMessage(message, message.SessionId.ToString());
            }
        }

        public void SendMessage(SimpleMessage message, string sessionId)
        {
            var webSockets = _wsClients.Where(s =>
            {
                var httpCookie = s.WebSocketContext.Cookies["SessionId"];
                return httpCookie != null && httpCookie.Value == sessionId;
            });

            foreach (var socket in webSockets)
            {
                socket.Send(JsonConvert.SerializeObject(message));
            }
        }

        public void SendBroadcastMessage(SimpleMessage message)
        {
            _wsClients.Broadcast(JsonConvert.SerializeObject(message));
        }
    }
}
