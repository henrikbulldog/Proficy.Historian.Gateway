using Newtonsoft.Json;
using Proficy.Historian.ClientAccess.API;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;

namespace Proficy.Historian.Client
{
    public class HistorianClient : IHistorian
    {
        private Dictionary<string, string> messageProperties = new Dictionary<string, string>();
        private ServerConnection historian;
        private HistorianClientConfiguration config;
        private Queue<SensorData> sensorDataQueue = new Queue<SensorData>();

        public HistorianClient(HistorianClientConfiguration config)
        {
            this.config = config;
            messageProperties.Add("source", "Proficy Historian Gateway");
            messageProperties.Add("name", "data");
        }

        public bool Start()
        {
            Log.Information("Proficy Historian Gateway starting up...");
            try
            {
                Log.Information($"Proficy Historian Gateway connecting to {config.ServerName} ...");
                historian = new ServerConnection(new ConnectionProperties { ServerHostName = config.ServerName, Username = config.UserName, Password = config.Password, ServerCertificateValidationMode = CertificateValidationMode.None });
                historian.Connect();
                historian.DataChangedEvent += new DataChangedHandler(Historian_DataChangedEvent);
                if (config.SubscribeMessage != null)
                {
                    var configEvent = new ConfigurationEvent();
                    configEvent.SubscribeMessage = config.SubscribeMessage;
                    Handle(configEvent);
                }
                Log.Information("Proficy Historian client started...");
            }
            catch (Exception ex)
            {
                Log.Error("Proficy Historian client - Error while initializing: " + ex.Message);
                return false;
            }

            return true;
        }

        public void Historian_DataChangedEvent(List<CurrentValue> values)
        {
            var sensorData = new List<SensorData>();

            foreach (CurrentValue cv in values)
            {
                double value;
                if (double.TryParse(cv.Value.ToString(), out value))
                {
                    sensorData.Add(new SensorData(cv.Tagname, value, new DateTimeOffset(cv.Time).ToUnixTimeMilliseconds(), cv.Quality.Status.ToString()));
                }
                else
                {
                    Log.Error($"Tag skipped. Can only publish numeric values: {JsonConvert.SerializeObject(cv)}");
                }
            }

            if (sensorData.Count > 0)
            {
                var sensorDataEvent = new SensorDataEvent(sensorData);
                DomainEvents.Raise(sensorDataEvent);
            }
        }

        public bool Stop()
        {
            Log.Information("Proficy Historian Gateway closing.");

            try
            {
                historian.IData.DropSubscriptions();
                historian.DataChangedEvent -= Historian_DataChangedEvent;

                historian.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Information("Proficy Historian Gateway - error while disposing: " + ex.Message);
            }

            Log.Information("Proficy Historian Gateway closed.");
            return true;
        }

        public void Handle(ConfigurationEvent configurationEvent)
        {
            if (configurationEvent != null)
            {
                if (configurationEvent.SubscribeMessage != null)
                {
                    foreach (var tag in configurationEvent.SubscribeMessage.Tags)
                    {
                        Log.Information("Proficy Historian Gateway - subscribing to " + tag.TagName);
                        try
                        {
                            historian.IData.Subscribe(new DataSubscriptionInfo
                            {
                                Tagname = tag.TagName,
                                MinimumElapsedMilliSeconds = tag.MinimumElapsedMilliSeconds.HasValue ? tag.MinimumElapsedMilliSeconds.Value : 0
                            });
                        }
                        catch (Exception exc)
                        {
                            Log.Information($"Could not subscribe to {tag.TagName}. {exc}");
                        }
                    }
                }

                if (configurationEvent.UnsubscribeMessage != null)
                {
                    foreach (var tagname in configurationEvent.UnsubscribeMessage.Tagnames)
                    {
                        Log.Information("Proficy Historian Gateway - unsubscribing to " + tagname);
                        try
                        {
                            historian.IData.Unsubscribe(tagname);
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