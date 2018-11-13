using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SensorDataEventQueue { get; set; }
        public string ConfigurationEventQueue { get; set; }
    }
}
