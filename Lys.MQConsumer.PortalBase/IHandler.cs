using System.Threading.Tasks;

namespace Lys.MQConsumer.PortalBase
{
    public interface IHandler
    {
        Task<bool> RunAsync(string message);
    }
}