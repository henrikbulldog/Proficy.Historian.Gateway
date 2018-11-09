using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Serilog;

namespace Proficy.Historian.Gateway.Mock
{
    public class SensorDataLogger : IDomainEventHandler<SensorDataEvent>
    {
        public void Handle(SensorDataEvent sensorDataEvent)
        {
            Log.Information("Handling sensor data event: "
                + JsonConvert.SerializeObject(sensorDataEvent, Formatting.None));
        }
    }
}
