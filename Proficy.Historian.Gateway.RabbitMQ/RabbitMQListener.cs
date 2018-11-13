using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Service;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;

namespace Proficy.Historian.Gateway.RabbitMQ
{
    public class RabbitMQListener : IService
    {
        private string hostname;
        private string username;
        private string password;
        private string queue;
        private IConnection connection;
        private IModel channel;

        public RabbitMQListener(
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

        private IBasicConsumer GetConsumer()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Registered += (a, b) =>
            {
                try
                {
                    Console.WriteLine(b.ConsumerTag);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


            };

            consumer.Received += (model, ea) =>
            {

                OnNewEvent(model, ea);
            };
            return consumer;
        }

        private void OnNewEvent(object model, BasicDeliverEventArgs ea)
        {
            if (ea != null && ea.Body != null)
            {
                var body = System.Text.Encoding.UTF8.GetString(ea.Body);
                try
                {
                    var configurationEvent = JsonConvert.DeserializeObject<ConfigurationEvent>(body);
                    if(configurationEvent == null)
                    {
                        throw new Exception("Invalid configuration event format");
                    }
                    DomainEvents.Raise(configurationEvent);
                }
                catch (Exception exc)
                {
                    Log.Error($"Error recieving configuration event. {exc}");
                }
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
                channel.BasicConsume(queue: queue,
                    autoAck: true,
                    consumer: GetConsumer());
                Log.Information("RabbitMQ listener started...");
            }
            catch (Exception ex)
            {
                Log.Error($"RabbitMQ listener - Error while connecting to {hostname}:  {ex}");
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
