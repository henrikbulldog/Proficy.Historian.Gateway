using Newtonsoft.Json;
using Proficy.Historian.ClientAccess.API;
using Proficy.Historian.Gateway.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Client
{
    public class HistorianClient : IService
    {
        private Dictionary<string, string> _messageProperties = new Dictionary<string, string>();
        private ServerConnection _historian;
        private HistorianClientConfiguration _config;
        private IPublisher _publisher;

        public HistorianClient(IPublisher publisher, HistorianClientConfiguration config)
        {
            _publisher = publisher;
            _config = config;
            _messageProperties.Add("source", "GE Proficy Listener");
            _messageProperties.Add("name", "data");
        }

        public IService Start()
        {
            Console.WriteLine("GE Proficy Listener starting up...");
            try
            {
                _historian = new ServerConnection(new ConnectionProperties { ServerHostName = _config.ServerName, Username = _config.UserName, Password = _config.Password, ServerCertificateValidationMode = CertificateValidationMode.None });
                _historian.Connect();
                _historian.DataChangedEvent += new DataChangedHandler(Historian_DataChangedEvent);
                foreach (ProficyHistorianTag tag in _config.TagsToSubscribe)
                {
                    _historian.IData.Subscribe(new DataSubscriptionInfo { Tagname = tag.TagName, MinimumElapsedMilliSeconds = tag.MinimumElapsedMilliSeconds });
                    Console.WriteLine("GE Proficy Listener - subscribing to " + tag.TagName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GE Proficy Listener - Error while initializing: " + ex.Message);
                Stop();
            }

            Console.WriteLine("GE Proficy Listener started...");
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
            _publisher.SendMessage(message);

            Console.WriteLine("GE Proficy Listener - sent: "
                + JsonConvert.SerializeObject(message, Formatting.None));
        }

        public IService Stop()
        {
            Console.WriteLine("GE Proficy Listener closing.");

            try
            {
                _historian.IData.DropSubscriptions();
                _historian.DataChangedEvent -= Historian_DataChangedEvent;

                _historian.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GE Proficy Listener - error while disposing: " + ex.Message);
            }

            Console.WriteLine("GE Proficy Listener closed.");
            return this;
        }
    }
}