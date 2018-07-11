using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Shared
{
    public class UnsubscribeMessage
    {
        public List<string> Tagnames { get; set; }
    }
}
