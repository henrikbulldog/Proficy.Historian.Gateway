using Newtonsoft.Json;
using Proficy.Historian.Gateway.DomainEvent;
using Proficy.Historian.Gateway.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proficy.Historian.Gateway.RabbitMQ
{
    public class RabbitMQPublisher : IDomainEventHandler
    {
        private RabbitMQConfiguration config;

        public RabbitMQPublisher(RabbitMQConfiguration config)
        {
            this.config = config;
        }

        public void Handle(IDomainEvent domainEvent)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "SensorDataEvent",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent));

                channel.BasicPublish(exchange: "",
                                     routingKey: "SensorDataEvent",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}