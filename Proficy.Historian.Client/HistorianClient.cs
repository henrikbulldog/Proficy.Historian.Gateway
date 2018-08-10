using Newtonsoft.Json;
using Proficy.Historian.ClientAccess.API;
using Proficy.Historian.Gateway.Shared;
using Serilog;
using System;
using System.Collections.Generic;

namespace Proficy.Historian.Client
{
    public class HistorianClient : IHistorian
    {
        private Dictionary<string, string> _messageProperties = new Dictionary<string, string>();
        private ServerConnection _historian;
        private HistorianClientConfiguration _config;

        public IPublisher Publisher { get; set; }
        
        public HistorianClient(IPublisher publisher, HistorianClientConfiguration config)
        {
            Publisher = publisher;
            Publisher.OnMessage = (message) => Message(JsonConvert.DeserializeObject<HistorianMessage>(message));
            _config = config;
            _messageProperties.Add("source", "Proficy Historian Gateway");
            _messageProperties.Add("name", "data");
        }

        public IService Start()
        {
            Log.Information("Proficy Historian Gateway starting up...");
            try
            {
                _historian = new ServerConnection(new ConnectionProperties { ServerHostName = _config.ServerName, Username = _config.UserName, Password = _config.Password, ServerCertificateValidationMode = CertificateValidationMode.None });
                _historian.Connect();
                _historian.DataChangedEvent += new DataChangedHandler(Historian_DataChangedEvent);
            }
            catch (Exception ex)
            {
                Log.Information("Proficy Historian Gateway - Error while initializing: " + ex.Message);
                Stop();
            }

            Log.Information("Proficy Historian Gateway started...");
            return this;
        }

        public void Historian_DataChangedEvent(List<CurrentValue> values)
        {
            foreach (CurrentValue cv in values)
            {
                PublishTag(cv.Tagname, cv.Value.ToString(), cv.Time, cv.Quality.ToString());
            }
        }

        public void PublishTag(string TagName, string TagValue, DateTime TagDateTime, string TagQuality)
        {
            var message = new SensorData(TagName, TagValue, TagDateTime.ToString("yyyy-MM-dd HH:mm:ss"), TagQuality);
            if(Publisher != null)
            {
                Publisher.SendMessage(JsonConvert.SerializeObject(message, Formatting.None));
            }

            Log.Information("Proficy Historian Gateway - sent: "
                + JsonConvert.SerializeObject(message, Formatting.None));
        }

        public IService Stop()
        {
            Log.Information("Proficy Historian Gateway closing.");

            try
            {
                _historian.IData.DropSubscriptions();
                _historian.DataChangedEvent -= Historian_DataChangedEvent;

                _historian.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Information("Proficy Historian Gateway - error while disposing: " + ex.Message);
            }

            Log.Information("Proficy Historian Gateway closed.");
            return this;
        }
 
        public void Message(HistorianMessage message)
        {
            if (message != null)
            {
                if (message.SubscribeMessage != null)
                {
                    foreach (var tag in message.SubscribeMessage.Tags)
                    {
                        Log.Information("Proficy Historian Gateway - subscribing to " + tag.TagName);
                        try
                        {
                            _historian.IData.Subscribe(new DataSubscriptionInfo
                            {
                                Tagname = tag.TagName,
                                MinimumElapsedMilliSeconds = tag.MinimumElapsedMilliSeconds
                            });
                        }
                        catch(Exception exc)
                        {
                            Log.Information($"Could not subscribe to {tag.TagName}. {exc}");
                        }
                    }
                }
                if (message.UnsubscribeMessage != null)
                {
                    foreach (var tagname in message.UnsubscribeMessage.Tagnames)
                    {
                        Log.Information("Proficy Historian Gateway - unsubscribing to " + tagname);
                        try
                        {
                            _historian.IData.Unsubscribe(tagname);
                        }
                        catch (Exception exc)
                        {
                            Log.Information($"Could not unsubscribe to {tagname}. {exc}");
                        }
                    }
                }
            }
        }
    }
}