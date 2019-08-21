namespace Lys.MQConsumer.PortalBase.Settings
{
    public class RabbitMQSetting
    {
        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string QueueName { get; set; }

        public ushort PrefetchCount { get; set; }

        public bool IsDelayQueue { get; set; }

        public string Exchange { get; set; }

        public string ExchangeType { get; set; }
        
        public string RoutingDelayKey { get; set; }

        public int ConsumerCount { get; set; } = 1;
    }
}
