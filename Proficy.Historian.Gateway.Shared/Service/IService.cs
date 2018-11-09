using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    public interface IService
    {
        bool Start();
        bool Stop();
    }
}
