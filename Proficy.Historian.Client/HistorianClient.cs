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
        private ProficyHistorianConfiguration _config;
        private IPublisher _publisher;

        public HistorianClient(IPublisher publisher)
        {
            _publisher = publisher;

            Console.WriteLine("GE Proficy Listener initializing...");

            try
            {
                string configurationString = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("historian-config.json"));
                _config = JsonConvert.DeserializeObject<ProficyHistorianConfiguration>(configurationString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GE Proficy Listener not initialized - error: " + ex.Message);
                Stop();
            }

            _messageProperties.Add("source", "GE Proficy Listener");
            _messageProperties.Add("name", "data");

            Console.WriteLine("GE Proficy Listener initialized.");
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

            if (_config.PrintToConsole)
            {
                Console.WriteLine("GE Proficy Listener - sent: "
                    + JsonConvert.SerializeObject(message, Formatting.None));
            }
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

    public class MessageWrap
    {
        public string name = "Historian";
        public SensorData[] content = new SensorData[1];
    }

    public class SensorData
    {
        public string t;
        public string v;
        public string dt;
        public string q;

        public SensorData(string TagName, string TagValue, string TagDateTime, string TagQuality)
        {
            this.t = TagName;
            this.v = TagValue;
            this.dt = TagDateTime;
            this.q = TagQuality;
        }
    }

    public class ProficyHistorianConfiguration
    {
        public string ServerName;
        public string UserName;
        public string Password;
        public bool PrintToConsole = false;
        public IList<ProficyHistorianTag> TagsToSubscribe;
    }

    public class ProficyHistorianTag
    {
        public string TagName;
        public int MinimumElapsedMilliSeconds = 1000;
    }
}