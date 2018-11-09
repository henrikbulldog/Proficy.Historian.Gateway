using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Service;

namespace Proficy.Historian.Gateway.Interfaces
{
    public interface IHistorian : IService, IDomainEventHandler<ConfigurationEvent>
    {
    }
}
