using Proficy.Historian.Gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.DomainEvent

{
    public class ConfigurationEvent : IDomainEvent
    {
        public SubscribeMessage SubscribeMessage { get; set; }
        public UnsubscribeMessage UnsubscribeMessage { get; set; }
    }
}
