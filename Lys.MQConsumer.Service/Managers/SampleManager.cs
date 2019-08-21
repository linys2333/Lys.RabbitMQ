using Lys.MQConsumer.Service.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lys.MQConsumer.Service.Managers
{
    public class SampleManager : BaseService
    {
        private readonly ILogger m_Logger;

        public SampleManager(ILogger logger)
        {
            m_Logger = logger;
        }

        public async Task SaveAsync(string data)
        {
            m_Logger.LogInformation($"UserId：{UserId}，Data：{data}");
        }
    }
}
