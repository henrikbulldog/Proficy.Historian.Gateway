using Newtonsoft.Json;
using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace Proficy.Historian.Gateway.Mock
{
    class HistorianClientMock : IHistorian
    {
        private List<string> _tags;
        private bool _stop = false;

        public IPublisher Publisher { get; set; }

        public HistorianClientMock(IPublisher publisher)
        {
            Publisher = publisher;
            Publisher.OnMessage = (message) => Message(JsonConvert.DeserializeObject<HistorianMessage>(message));
            _tags = new List<string>();
        }

        public void Message(HistorianMessage message)
        {
            try
            {
                if (message != null)
                {
                    if (message.SubscribeMessage != null)
                    {
                        foreach (var tag in message.SubscribeMessage.Tags)
                        {
                            _tags.Add(tag.TagName);
                            Log.Information("Proficy Historian Gateway - subscribing to " + tag.TagName);
                        }
                    }
                    if (message.UnsubscribeMessage != null)
                    {
                        foreach (var tagname in message.UnsubscribeMessage.Tagnames)
                        {
                            if (_tags.Contains(tagname))
                            {
                                _tags.Remove(tagname);
                                Log.Information("Proficy Historian Gateway - unsubscribing to " + tagname);
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                Log.Error(exc, "Error recieving Historian maessage");
            }
        }

        private void OnMessage(string message)
        {
            var historianMessage = JsonConvert.DeserializeObject<HistorianMessage>(message);
            if (historianMessage != null)
            {
                Message(historianMessage);
            }
        }

        public IService Start()
        {
            new Thread(delegate ()
            {
                while (!_stop)
                {
                    if (Publisher != null)
                    {
                        foreach (var tag in _tags)
                        {
                            Publisher.SendMessage($"{tag} : {DateTime.Now}");
                            Log.Information($"{tag} : {DateTime.Now}");
                        }
                    }

                    Thread.Sleep(1000);
                }
            }).Start();
            return this;
        }

        public IService Stop()
        {
            _stop = true;
            return this;
        }
    }
}
