using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.DomainEvent
{
    public interface IDomainEventHandler<T>
        where T: IDomainEvent
    {
        void Handle(T domainEvent);
    }
}
