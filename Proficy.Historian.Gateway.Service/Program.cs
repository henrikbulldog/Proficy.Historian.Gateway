using System;
using Topshelf;
using Newtonsoft.Json;
using System.IO;
using Serilog;
using Proficy.Historian.WebSocket;
using Proficy.Historian.Gateway.Interfaces;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.RabbitMQ;

namespace Proficy.Historian.Gateway.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.EventLog("Proficy.Historian.Gateway", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            string configurationString = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("config.json"));
            var config = JsonConvert.DeserializeObject<Config>(configurationString);
            IHistorian historian = null;
#if DEBUG
            DomainEvents.Register<SensorDataEvent>(new Mock.SensorDataHandler());
            DomainEvents.Register<SensorDataEvent>(new RabbitMQPublisher(config.RabbitMQConfiguration));
            historian = new Mock.HistorianClient(
                config.HistorianClientConfiguration == null 
                    ? null 
                    : config.HistorianClientConfiguration.SubscribeMessage);
#else
            historian = new Client.HistorianClient(config.HistorianClientConfiguration));
#endif
            DomainEvents.Register<ConfigurationEvent>(historian);

            var rc = HostFactory.Run(x =>
            {
                x.Service<IService>(service =>
                {
                    service.ConstructUsing(hostSettings =>
                    new ServiceManager()
                        .Add(historian)
                        .Add(new WebSocketService(config.WebSocketServiceConfiguration)));
                    service.WhenStarted(sm => sm.Start());
                    service.WhenStopped(sm => sm.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("Proficy.Historian.Gateway.Service");
                x.SetDisplayName("GE Proficy Historian Gateway Service");
                x.SetDescription("Exposes GE Proficy Historian events through Web Sockets");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
