using Lys.MQConsumer.Biz.Portal.DTOs;
using Lys.MQConsumer.PortalBase;
using Lys.MQConsumer.Service.Managers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lys.MQConsumer.Biz.Portal
{
    public class MainHandler : MainHandlerBase<BizDto>
    {
        private readonly ILogger m_Logger;
        private readonly TestManager m_TestManager;

        public MainHandler(ILogger logger, TestManager testManager) : base(logger)
        {
            m_Logger = logger;
            m_TestManager = testManager;
        }
        
        protected override async Task<bool> RunAsync(BizDto bizDto)
        {
            await m_TestManager.SaveAsync(bizDto.FirmId);
            return true;
        }
    }
}