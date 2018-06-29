using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Lys.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            
            using (var channel = connectFactory.CreateConnection().CreateModel())
            {
                channel.QueueDeclare(queue: "test", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicPublish(exchange: "", routingKey: "test", basicProperties: null,
                    body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                    {
                        UserId = "386E8A2F-8C51-4958-BA34-60CF6F521D03",
                        FirmId = "00000000-0000-0000-0000-000000000000"
                    })));
            }
        }
    }
}
