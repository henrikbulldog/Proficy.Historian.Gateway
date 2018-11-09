using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using Proficy.Historian.Gateway.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Proficy.Historian.Gateway.Mock
{
    public class HistorianClientMock : IHistorian
    {
        private List<string> tags;
        private bool stop = false;

        public HistorianClientMock(SubscribeMessage subscribeMessage = null)
        {
            tags = new List<string>();
            if (subscribeMessage != null)
            {
                tags.AddRange(subscribeMessage.Tags.Select(t => t.TagName));
            }
        }

        public void Handle(ConfigurationEvent configurationEvent)
        {
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

        public bool Start()
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
                Thread.Sleep(1000);
            }

            return true;
        }

        public bool Stop()
        {
            lock (this)
            {
                stop = true;
            }
            return true;
        }
    }
}
