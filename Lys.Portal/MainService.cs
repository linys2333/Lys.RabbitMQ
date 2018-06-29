using log4net;
using Lys.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Lys.Portal
{
    public class MainService : IMicroService
    {
        private readonly IServiceProvider m_Services;
        private readonly IConfiguration m_Config;
        private readonly ILog m_Logger;
        private readonly CancellationTokenSource m_CancellationToken;

        public MainService(IServiceProvider services, IConfiguration config, ILog log)
        {
            m_Services = services;
            m_Config = config;
            m_Logger = log;
            m_CancellationToken = new CancellationTokenSource();
        }

        public void Start()
        {
            try
            {
                var connectFactory = new ConnectionFactory
                {
                    HostName = m_Config["RabbitMQ:HostName"],
                    UserName = m_Config["RabbitMQ:UserName"],
                    Password = m_Config["RabbitMQ:Password"],
                    VirtualHost = m_Config["RabbitMQ:VirtualHost"],
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                var connection = connectFactory.CreateConnection();
                var channel = connection.CreateModel();
                var queueName = m_Config["RabbitMQ:QueueName"];

                m_CancellationToken.Token.Register(() =>
                {
                    m_Logger.Info("关闭 RabbitMQ 连接");
                    connection.Close();
                });

                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: ushort.Parse(m_Config["RabbitMQ:PrefetchCount"]), global: false);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += async (sender, e) =>
                {
                    var message = Encoding.UTF8.GetString(e.Body);

                    try
                    {
                        // 每次都要实例化
                        var handler = m_Services.GetRequiredService<IHandler>();

                        var sw = Stopwatch.StartNew();
                        var result = await handler.RunAsync(message);
                        sw.Stop();
                        m_Logger.Info($"处理耗时：{sw.ElapsedMilliseconds}ms");

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
                        m_Logger.Error(ex);
                        throw;
                    }
                };

                m_Logger.Info("启动 RabbitMQ 接收");
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex);
                throw;
            }
        }

        public void Stop()
        {
            m_CancellationToken.Cancel();
        }
    }
}