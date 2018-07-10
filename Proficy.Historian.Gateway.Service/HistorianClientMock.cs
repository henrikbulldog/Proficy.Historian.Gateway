using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    class HistorianClientMock : IService
    {
        private IPublisher _publisher;
        private bool _stop = false;
        public HistorianClientMock(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public IService Start()
        {
            new Thread(delegate ()
            {
                while (!_stop)
                {
                    _publisher.SendMessage($"Ping at {DateTime.Now}");
                    Thread.Sleep(5000);
                }
            }).Start();
            return this;
        }

        public IService Stop()
        {
            _stop = true;
            return this;
        }
    }
}
