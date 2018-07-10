using System;
using Topshelf;
using Proficy.Historian.WebSocket;
using Proficy.Historian.Client;

namespace Proficy.Historian.Gateway.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<ServiceManager>(s =>
                {
                    var webSocketService = new WebSocketService("ws://0.0.0.0:15099");
                    s.ConstructUsing(name => new ServiceManager()
                        .Add(webSocketService)
#if DEBUG
                        .Add(new HistorianClientMock(webSocketService))
#else
                        .Add(new HistorianClient(webSocketService))
#endif
                        );
                    s.WhenStarted(sm => sm.Start());
                    s.WhenStopped(sm => sm.Stop());
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
