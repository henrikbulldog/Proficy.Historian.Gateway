using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using Proficy.Historian.Gateway.Models;
using Proficy.Historian.Gateway.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Proficy.Historian.Gateway.Mock
{
    public class HistorianClient : IHistorian
    {
        private List<string> tags;
        private bool stop = false;

        public HistorianClient(SubscribeMessage subscribeMessage = null)
        {
            tags = new List<string>();
            if(subscribeMessage != null)
            {
                tags.AddRange(subscribeMessage.Tags.Select(t => t.TagName));
            }
        }

        public void Handle(IDomainEvent domainEvent)
        {
            var configurationEvent = domainEvent as ConfigurationEvent;
            try
            {
                if (configurationEvent != null)
                {
                    if (configurationEvent.SubscribeMessage != null)
                    {
                        foreach (var tag in configurationEvent.SubscribeMessage.Tags)
                        {
                            tags.Add(tag.TagName);
                            Log.Information("Proficy Historian Gateway - subscribing to " + tag.TagName);
                        }
                    }
                    if (configurationEvent.UnsubscribeMessage != null)
                    {
                        foreach (var tagname in configurationEvent.UnsubscribeMessage.Tagnames)
                        {
                            if (tags.Contains(tagname))
                            {
                                tags.Remove(tagname);
                                Log.Information("Proficy Historian Gateway - unsubscribing to " + tagname);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error recieving Historian maessage");
            }
        }

        public IService Start()
        {
            new Thread(delegate ()
            {
                while (!stop)
                {
                    var data = new List<SensorData>();
                    foreach (var tag in tags)
                    {
                        data.Add(new SensorData(
                            tag,
                            DateTime.Now.Ticks,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            "Good"));
                    }
                    var sensorDataEvent = new SensorDataEvent(data);
                    DomainEvents.Raise(sensorDataEvent);
                    Thread.Sleep(2000);
                }
            }).Start();
            return this;
        }

        public IService Stop()
        {
            stop = true;
            return this;
        }
    }
}
