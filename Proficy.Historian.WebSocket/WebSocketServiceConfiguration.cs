using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.WebSocket
{
    public class WebSocketServiceConfiguration
    {
        private string _address;

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                var env = Environment.GetEnvironmentVariable(value);
                if (env != null)
                {
                    _address = env;
                }
                else
                {
                    _address = value;
                }
            }
        }
    }
}
