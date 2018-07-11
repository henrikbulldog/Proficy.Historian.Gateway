﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Shared
{
    public interface IPublisher
    {
        Action<string> OnMessage { get; set; }

        IHistorian Historian { get; set; }

        void SendMessage(object message);
    }
}
