using System.Collections.Generic;
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

        public bool Start()
        {
            foreach (var service in services)
            {
                Task.Run(() => service.Start());
            }
            return true;
        }

        public bool Stop()
        {
            var r = true;
            foreach (var service in services)
            {
                if (!service.Stop())
                {
                    r = false;

                }
            }
            return r;
        }
    }
}
