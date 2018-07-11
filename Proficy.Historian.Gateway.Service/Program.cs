using System;
using Topshelf;
using Proficy.Historian.WebSocket;
using Proficy.Historian.Client;
using Newtonsoft.Json;
using System.IO;
using Proficy.Historian.Gateway.Shared;

namespace Proficy.Historian.Gateway.Service
{
    class Program
    {
        static private IHistorian _historian;

        /// <summary>
        /// Send a message to the historian to subscribe or unsuscribe to tag data changes
        /// </summary>
        /// <param name="message">HistorianMessage as a json object</param>
        static void OnMessage(string message)
        {
            if (_historian != null)
            {
                var historianMessage = JsonConvert.DeserializeObject<HistorianMessage>(message);
                if (historianMessage != null)
                {
                    _historian.Message(historianMessage);
                }
            }
        }

        static void Main(string[] args)
        {
            string configurationString = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("config.json"));
            var config = JsonConvert.DeserializeObject<Config>(configurationString);

            var rc = HostFactory.Run(x =>
            {
                x.Service<IService>(service =>
                {
                    service.ConstructUsing(hostSettings => new WebSocketService(
                        config.WebSocketServiceConfiguration,
                        (publisher) =>
                        {
#if DEBUG
                            publisher.Historian = new HistorianClientMock(publisher);
#else
                            publisher.Historian = new HistorianClient(publisher, config.HistorianClientConfiguration);
#endif
                            publisher.Historian.Start();
                        }));
                    service.WhenStarted(sm => sm.Start());
                    service.WhenStopped(sm => sm.Stop());
                });
                x.RunAsLocalSystem();

                x.SetServiceName("Proficy.Historian.Gateway.Service");
                x.SetDisplayName("GE Proficy Historian Gateway Service");
                x.SetDescription("Exposes GE Proficy Historian events as a web API");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
