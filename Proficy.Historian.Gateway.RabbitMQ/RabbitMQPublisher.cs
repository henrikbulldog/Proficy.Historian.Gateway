using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Service;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Text;

namespace Proficy.Historian.Gateway.RabbitMQ
{
    public class RabbitMQPublisher : IService, IDomainEventHandler<SensorDataEvent>
    {
        private string hostname;
        private string username;
        private string password;
        private string queue;
        private IConnection connection;
        private IModel channel;

        public RabbitMQPublisher(
            string hostname,
            string username,
            string password,
            string queue)
        {
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.queue = queue;
        }

        public void Handle(SensorDataEvent sensorDataEvent)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sensorDataEvent));

                if (channel != null)
                {
                    channel.BasicPublish(exchange: "",
                                         routingKey: queue,
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch (Exception exc)
            {
                Log.Error($"Could not send to queue {queue}. {JsonConvert.SerializeObject(sensorDataEvent)}. {exc}.");
            }
        }

        public bool Start()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = hostname,
                    UserName = username,
                    Password = password,
                    NetworkRecoveryInterval = new TimeSpan(0, 0, 0, 5),
                    AutomaticRecoveryEnabled = true
                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare(queue: queue,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);
                Log.Information("RabbitMQ publisher started...");
            }
            catch (Exception ex)
            {
                Log.Error($"RabbitMQ publisher - Error while connecting to {hostname}:  {ex}");
            }

            return true;
        }

        public bool Stop()
        {
            if (connection != null)
            {
                connection.Close();
            }
            return true;
        }
    }
}