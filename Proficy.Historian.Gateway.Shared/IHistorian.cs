using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Shared
{
    public interface IHistorian : IService
    {
        IPublisher Publisher { get; set; }

        void Message(HistorianMessage message);
    }
}
