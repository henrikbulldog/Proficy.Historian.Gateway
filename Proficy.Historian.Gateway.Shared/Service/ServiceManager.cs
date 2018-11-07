using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    public class ServiceManager : IService
    {
        private List<IService> services = new List<IService>();

        public ServiceManager Add(IService service)
        {
            services.Add(service);
            return this;
        }

        public ServiceManager Remove(IService service)
        {
            services.Remove(service);
            return this;
        }

        public IService Start()
        {
            foreach(var service in services)
            {
                service.Start();
            }
            return this;
        }

        public IService Stop()
        {
            foreach (var service in services)
            {
                service.Stop();
            }
            return this;
        }
    }
}
