using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Mock
{
    public class SensorDataHandler : IDomainEventHandler
    {
        public void Handle(IDomainEvent e)
        {
            Log.Information("Handling sensor data event: "
                + JsonConvert.SerializeObject(e, Formatting.None));
        }
    }
}
