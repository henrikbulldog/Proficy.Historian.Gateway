using Proficy.Historian.Gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Client
{
    public class HistorianClientConfiguration
    {
        public string ServerName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public SubscribeMessage SubscribeMessage { get; set; }
    }
}
