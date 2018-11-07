using System.Collections.Generic;

namespace Proficy.Historian.Gateway.DomainEvent
{
    public class SensorDataEvent : IDomainEvent
    {
        public List<SensorData> SensorData { get; private set; }

        public SensorDataEvent(List<SensorData> sensorData)
        {
            SensorData = sensorData;
        }
    }
}
