using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace Proficy.Historian.Gateway.Api
{
    public class MessagesController : ApiController
    {
        private readonly WebSocketHandler _webSocketHandler;

        public MessagesController(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        [HttpGet]
        [Route("api/ping")]
        public IHttpActionResult Ping()
        {
            return Ok($"Pong at {DateTime.Now}");
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var currentContext = HttpContext.Current;
            return await Task.Run(() =>
            {
                if (currentContext.IsWebSocketRequest || currentContext.IsWebSocketRequestUpgrading)
                {
                    currentContext.AcceptWebSocketRequest(ProcessWebSocketRequest);
                    return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            });

        }

        private async Task ProcessWebSocketRequest(AspNetWebSocketContext context)
        {
            var sessionCookie = context.Cookies["SessionId"];
            if (sessionCookie != null)
            {
                await _webSocketHandler.ProcessWebSocketRequestAsync(context);
            }
        }
    }
}
