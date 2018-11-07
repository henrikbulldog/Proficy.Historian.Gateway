using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Models
{
    public class HistorianTag
    {
        public string TagName;
        public int MinimumElapsedMilliSeconds = 1000;
    }
}
