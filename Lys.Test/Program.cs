using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Lys.Test
{
    class Program
    {
        private static ConnectionFactory m_ConnectionFactory;
         
        static void Main(string[] args)
        {
            m_ConnectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            Enqueue("test", new
            {
                UserId = "00000000-0000-0000-0000-000000000000",
                SampleData = "消息1"
            });

            DelayEnqueue("exchange-direct", "DL-routing-delay", new
            {
                UserId = "00000000-0000-0000-0000-000000000000",
                SampleData = "消息2"
            }, 5000);

            Console.WriteLine("发送完毕");
        }

        public static void Enqueue(string queueName, object data)
        {
            using (var channel = m_ConnectionFactory.CreateConnection().CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null,
                    body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            }
        }

        public static void DelayEnqueue(string exchange, string routeKey, object data, int delayMillisecond)
        {
            using (var channel = m_ConnectionFactory.CreateConnection().CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                properties.Expiration = delayMillisecond.ToString();

                channel.BasicPublish(exchange: exchange, routingKey: routeKey, basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            }
        }
    }
}
