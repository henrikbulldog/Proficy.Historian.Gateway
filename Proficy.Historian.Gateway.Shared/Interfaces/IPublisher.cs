using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Interfaces
{
    public interface IPublisher
    {
        void SendMessage(object message);
    }
}