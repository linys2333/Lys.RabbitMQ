using Lys.MQConsumer.Service.Common;
using Lys.MQConsumer.Service.Stores;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Lys.MQConsumer.Service.Managers
{
    public class TestManager
    {
        private readonly MySQLDbContext m_DbContext;

        public TestManager(MySQLDbContext dbContext)
        {
            m_DbContext = dbContext;
        }

        public async Task SaveAsync(string firmId)
        {
            var user = await m_DbContext.Users.FirstOrDefaultAsync(u => u.Id == BaseService.UserId);
            user.FirmId = firmId;
            await m_DbContext.SaveChangesAsync();
        }
    }
}
