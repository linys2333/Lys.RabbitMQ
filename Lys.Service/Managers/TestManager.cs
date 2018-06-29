using Lys.Service.Stores;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Lys.Service.Managers
{
    public class TestManager
    {
        private readonly MySQLDbContext m_DbContext;

        public TestManager(MySQLDbContext dbContext)
        {
            m_DbContext = dbContext;
        }

        public async Task SaveAsync(string userId, string firmId)
        {
            var user = await m_DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.FirmId = firmId;
            await m_DbContext.SaveChangesAsync();
        }
    }
}
