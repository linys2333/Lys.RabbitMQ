using Lys.MQConsumer.PortalBase.DTOs;
using Lys.MQConsumer.Service.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Lys.MQConsumer.PortalBase
{
    public abstract class MainHandlerBase<T> : IHandler where T : ConsumerInfo
    {
        private readonly ILogger m_Logger;

        public MainHandlerBase(ILogger logger)
        {
            m_Logger = logger;
        }

        public Task<bool> RunAsync(string message)
        {
            var consumerInfo = JsonConvert.DeserializeObject<T>(message);

            BaseService.Init(consumerInfo.UserId);

            return RunAsync(consumerInfo);
        }

        protected abstract Task<bool> RunAsync(T consumerInfo);
    }
}