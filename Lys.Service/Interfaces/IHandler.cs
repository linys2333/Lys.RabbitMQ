using System.Threading.Tasks;

namespace Lys.Service
{
    public interface IHandler
    {
        Task<bool> RunAsync(string message);
    }
}