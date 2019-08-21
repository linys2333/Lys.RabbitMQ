using Lys.MQConsumer.PortalBase;
using Lys.MQConsumer.Sample.Portal.DTOs;
using Lys.MQConsumer.Service.Managers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lys.MQConsumer.Sample.Portal
{
    public class MainHandler : MainHandlerBase<SampleDTO>
    {
        private readonly ILogger m_Logger;
        private readonly SampleManager m_SampleManager;

        public MainHandler(ILogger logger, SampleManager sampleManager) : base(logger)
        {
            m_Logger = logger;
            m_SampleManager = sampleManager;
        }
        
        protected override async Task<bool> RunAsync(SampleDTO sampleDto)
        {
            await m_SampleManager.SaveAsync(sampleDto.SampleData);
            return true;
        }
    }
}