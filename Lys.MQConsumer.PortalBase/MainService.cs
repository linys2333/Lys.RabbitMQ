using Lys.MQConsumer.PortalBase.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lys.MQConsumer.PortalBase
{
    public class MainService : IMicroService
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly RabbitMQSetting m_RabbitMQSetting;
        private readonly ILogger m_Logger;
        private readonly CancellationTokenSource m_CancellationToken;

        public MainService(IServiceProvider serviceProvider, RabbitMQSetting rabbitMQSetting, ILogger<MainService> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_RabbitMQSetting = rabbitMQSetting;
            m_Logger = logger;
            m_CancellationToken = new CancellationTokenSource();
        }

        public void Start()
        {
            try
            {
                var connectFactory = new ConnectionFactory
                {
                    HostName = m_RabbitMQSetting.HostName,
                    UserName = m_RabbitMQSetting.UserName,
                    Password = m_RabbitMQSetting.Password,
                    VirtualHost = m_RabbitMQSetting.VirtualHost,
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                var connection = connectFactory.CreateConnection();
                var channel = connection.CreateModel();
                m_CancellationToken.Token.Register(() =>
                {
                    m_Logger.LogInformation("关闭 RabbitMQ 连接");
                    channel.Close();
                    connection.Close();
                });

                var queueName = m_RabbitMQSetting.QueueName;

                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                if (m_RabbitMQSetting.IsDelayQueue)
                {
                    channel.QueueBind(queue: queueName, exchange: m_RabbitMQSetting.Exchange, routingKey: m_RabbitMQSetting.RoutingDelayKey);
                }
                channel.BasicQos(prefetchSize: 0, prefetchCount: m_RabbitMQSetting.PrefetchCount, global: false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += ConsumerOnReceived;

                for (var i = 0; i < m_RabbitMQSetting.ConsumerCount; i++)
                {
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }

                m_Logger.LogInformation($"启动 RabbitMQ 接收。Consumer数：{m_RabbitMQSetting.ConsumerCount}");
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "ServiceError");
                throw;
            }
        }
        
        public void Stop()
        {
            m_CancellationToken.Cancel();
        }

        protected virtual async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            var sw = Stopwatch.StartNew();

            var message = Encoding.UTF8.GetString(e.Body);

            Console.WriteLine(e.RoutingKey);

            try
            {
                var channel = (sender as IBasicConsumer).Model;

                var handler = m_ServiceProvider.GetRequiredService<IHandler>();
                var result = await handler.RunAsync(message);
                if (result)
                {
                    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                }
                else
                {
                    channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"HandlerError:{message}");
                throw;
            }
            finally
            {
                sw.Stop();

                m_Logger.LogInformation($"消息正文：{message}，耗时：{sw.ElapsedMilliseconds} ms");
            }
        }
    }
}