using Microsoft.Web.WebSockets;
using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.SelfHost;
using Unity;

namespace Proficy.Historian.Gateway.Api
{
    public class HttpApiService : IPublisher
    {
        private readonly HttpSelfHostServer _server;
        private readonly HttpSelfHostConfiguration _config;
        private const string EventSource = "HttpApiService";
        private SimpleWebSocketHandler _webSocketHandler;

        public HttpApiService(Uri address)
        {
            //if (!EventLog.SourceExists(EventSource))
            //{
            //    EventLog.CreateEventSource(EventSource, "Application");
            //}
            //EventLog.WriteEntry(EventSource, $"Creating server at {address.ToString()}");
            Console.WriteLine($"Creating server at {address.ToString()}");
            _config = new HttpSelfHostConfiguration(address);
            _config.MapHttpAttributeRoutes();
            _config.Formatters.Clear();
            _config.Formatters.Add(new JsonMediaTypeFormatter());
            _config.Routes.MapHttpRoute("API", 
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
            _config.DependencyResolver = Bootstrap();
            _server = new HttpSelfHostServer(_config);
        }

        private IDependencyResolver Bootstrap()
        {
            var container = new UnityContainer();
            _webSocketHandler = new SimpleWebSocketHandler();
            container.RegisterInstance<WebSocketHandler>(_webSocketHandler);
            return new UnityResolver(container);
        }

        public IService Start()
        {
            //EventLog.WriteEntry(EventSource, "Opening HttpApiService server.");
            Console.WriteLine("HTTP server starting");
            _server.OpenAsync();
            return this;
        }

        public IService Stop()
        {
            Console.WriteLine("HTTP server closing.");
            _server.CloseAsync().Wait();
            _server.Dispose();
            return this;
        }

        public void SendMessage(SimpleMessage message)
        {
            _webSocketHandler.SendMessage(message);
        }
    }
}
