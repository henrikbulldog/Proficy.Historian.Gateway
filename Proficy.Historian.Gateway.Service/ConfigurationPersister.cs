using Proficy.Historian.Gateway.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    public class ConfigurationPersister : IDomainEventHandler<ConfigurationEvent>
    {
        public void Handle(ConfigurationEvent configurationEvent)
        {
            if (configurationEvent == null)
            {
                return;
            }

            var config = Config.FromFile(replaceEnvironmentVariables: false);
            if (config.HistorianClientConfiguration == null)
            {
                config.HistorianClientConfiguration = new Client.HistorianClientConfiguration();
            }

            if (config.HistorianClientConfiguration.SubscribeMessage == null)
            {
                config.HistorianClientConfiguration.SubscribeMessage = new Models.SubscribeMessage();
            }

            if (configurationEvent.UnsubscribeMessage != null && configurationEvent.UnsubscribeMessage.Tagnames != null)
            {
                foreach (var tagname in configurationEvent.UnsubscribeMessage.Tagnames)
                {
                    var tag = config.HistorianClientConfiguration.SubscribeMessage.Tags.FirstOrDefault(t => t.TagName == tagname);
                    if (tag != null)
                    {
                        config.HistorianClientConfiguration.SubscribeMessage.Tags.Remove(tag);
                    }
                }
            }

            if (configurationEvent.SubscribeMessage != null && configurationEvent.SubscribeMessage.Tags != null)
            {
                foreach (var newtag in configurationEvent.SubscribeMessage.Tags)
                {
                    var tag = config.HistorianClientConfiguration.SubscribeMessage.Tags.FirstOrDefault(t => t.TagName == newtag.TagName);
                    if (tag != null)
                    {
                        config.HistorianClientConfiguration.SubscribeMessage.Tags.Remove(tag);
                    }
                    config.HistorianClientConfiguration.SubscribeMessage.Tags.Add(newtag);
                }
            }

            config.ToFile();
        }
    }
}
