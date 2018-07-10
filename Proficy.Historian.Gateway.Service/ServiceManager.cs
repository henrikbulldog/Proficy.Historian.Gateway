using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    class ServiceManager
    {
        private List<IService> services = new List<IService>();

        public ServiceManager Add(IService service)
        {
            services.Add(service);
            return this;
        }

        public ServiceManager Start()
        {
            foreach(var s in services)
            {
                s.Start();
            }
            return this;
        }

        public ServiceManager Stop()
        {
            foreach (var s in services)
            {
                s.Stop();
            }
            return this;
        }
    }
}
