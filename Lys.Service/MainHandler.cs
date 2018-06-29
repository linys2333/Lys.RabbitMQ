using log4net;
using Lys.Service.Dtos;
using Lys.Service.Managers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Lys.Service
{
    public class MainHandler : IHandler
    {
        private readonly ILog m_Log;
        private readonly TestManager m_TestManager;
        
        public MainHandler(ILog log, TestManager testManager)
        { 
            m_Log = log;
            m_TestManager = testManager;
        }

        public async Task<bool> RunAsync(string message)
        {
            m_Log.Info($"开始处理，message：{message}");

            var handoverInfo = JsonConvert.DeserializeObject<HandleDto>(message);

            Check(handoverInfo);
            var result = await RunAsync(handoverInfo);

            m_Log.Info(result ? $"处理完毕，message：{message}" : $"离职交接未完成，message：{message}");
            return result;
        }

        private void Check(HandleDto handleDto)
        {
            if (string.IsNullOrEmpty(handleDto.UserId) || string.IsNullOrEmpty(handleDto.FirmId))
            {
                throw new ArgumentException(JsonConvert.SerializeObject(handleDto));
            }
            
            m_Log.Info("step1 校验通过");
        }

        private async Task<bool> RunAsync(HandleDto handleDto)
        {
            m_Log.Info("step2 步骤1");

            await m_TestManager.SaveAsync(handleDto.UserId, handleDto.FirmId);

            return true;
        }
    }
}