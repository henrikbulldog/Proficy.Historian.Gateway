using Proficy.Historian.Client;
using Proficy.Historian.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    public class Config
    {
        public HistorianClientConfiguration HistorianClientConfiguration { get; set; }
        public WebSocketServiceConfiguration WebSocketServiceConfiguration { get; set; }
    }
}
