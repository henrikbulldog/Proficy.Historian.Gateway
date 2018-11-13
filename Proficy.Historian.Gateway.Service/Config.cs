using Newtonsoft.Json;
using Proficy.Historian.Client;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.RabbitMQ;
using Proficy.Historian.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.Service
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public class Config
    {
        public HistorianClientConfiguration HistorianClientConfiguration { get; set; }

        public WebSocketServiceConfiguration WebSocketServiceConfiguration { get; set; }

        public RabbitMQConfiguration RabbitMQConfiguration { get; set; }

        public static Config FromFile(string filename = "config.json", bool replaceEnvironmentVariables = true)
        {
            string configurationString = File.ReadAllText(filename);
            if(string.IsNullOrEmpty(configurationString))
            {
                return new Config();
            }
            var config = JsonConvert.DeserializeObject<Config>(configurationString);
            if(replaceEnvironmentVariables)
            {
                ReplaceEnvironmentVariables(config);
            }
            return config;
        }

        private static void ReplaceEnvironmentVariables(Config config)
        {
            ReplaceEnvironmentVariables(config.HistorianClientConfiguration);
            ReplaceEnvironmentVariables(config.WebSocketServiceConfiguration);
            ReplaceEnvironmentVariables(config.RabbitMQConfiguration);
        }

        private static void ReplaceEnvironmentVariables(object o)
        {
            if (o != null && o.GetType().GetProperties() != null)
            {
                foreach(var p in o.GetType().GetProperties())
                {
                    if (p.PropertyType == typeof(string))
                    {
                        var e = Environment.GetEnvironmentVariable(p.GetValue(o).ToString());
                        if (e != null)
                        {
                            p.SetValue(o, e);
                        }
                    }
                }
            }
        }

        public void ToFile(string filename = "config.json")
        {
            string configurationString = JsonConvert.SerializeObject(
                this, 
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            File.WriteAllText(filename, configurationString, Encoding.UTF8);
        }
    }
}
