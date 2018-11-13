using Proficy.Historian.Gateway.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.DomainEvent
{
    public class SensorData
    {
        public string Tagname;
        public double Value;
        public long Time;
        public string Quality;

        public SensorData(string TagName, double TagValue, long TagDateTime, string TagQuality)
        {
            this.Tagname = TagName;
            this.Value = TagValue;
            this.Time = TagDateTime;
            this.Quality = TagQuality;
        }
    }

}
