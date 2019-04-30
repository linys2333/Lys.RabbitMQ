using System.Threading;

namespace Lys.MQConsumer.Service.Common
{
    public abstract class BaseService
    {
        private static readonly AsyncLocal<string> m_AsyncUserId = new AsyncLocal<string>();

        public static string UserId => m_AsyncUserId.Value;

        public static void Init(string userId)
        {
            m_AsyncUserId.Value = userId;
        }
    }
}
