using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using Proficy.Historian.Gateway.RabbitMQ;
using Proficy.Historian.WebSocket;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;
using Topshelf;

namespace Proficy.Historian.Gateway.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();
                x.SetServiceName("Proficy.Historian.Gateway.Service");
                x.SetDisplayName("GE Proficy Historian Gateway Service");
                x.SetDescription("Forwards GE Proficy Historian data changed events through Web Sockets and a RabbitMQ queue");

                x.Service<ServiceManager>(service =>
                {
                    service.ConstructUsing(hostSettings => new ServiceManager());
                    service.WhenStarted(sm => OnStart(sm));
                    service.WhenStopped(sm => sm.Stop());
                });
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        private static bool OnStart(ServiceManager serviceManager)
        {
            var config = Config.FromFile();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.RollingFile(new JsonFormatter(), "log-{Date}.txt", retainedFileCountLimit: 30)
                .CreateLogger();

            IHistorian historian = null;
#if DEBUG
            historian = new Mock.HistorianClientMock(
                config.HistorianClientConfiguration == null
                    ? null
                    : config.HistorianClientConfiguration.SubscribeMessage);
#else
            historian = new Client.HistorianClient(config.HistorianClientConfiguration);
#endif
            DomainEvents.Register(historian);

            var rabbitMQPublisher = new RabbitMQPublisher(
                        config.RabbitMQConfiguration.Hostname,
                        config.RabbitMQConfiguration.Username,
                        config.RabbitMQConfiguration.Password,
                        config.RabbitMQConfiguration.Queue);
            DomainEvents.Register(rabbitMQPublisher);
#if DEBUG
            DomainEvents.Register(new Mock.SensorDataLogger());
#endif
            DomainEvents.Register(new ConfigurationPersister());

            return serviceManager
                .Add(rabbitMQPublisher)
                .Add(new WebSocketService(config.WebSocketServiceConfiguration))
                .Add(historian)
                .Start();
        }
    }
}
