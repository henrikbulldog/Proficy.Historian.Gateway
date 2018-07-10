using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Client
{
    public class SensorData
    {
        public string t;
        public string v;
        public string dt;
        public string q;

        public SensorData(string TagName, string TagValue, string TagDateTime, string TagQuality)
        {
            this.t = TagName;
            this.v = TagValue;
            this.dt = TagDateTime;
            this.q = TagQuality;
        }
    }

}
