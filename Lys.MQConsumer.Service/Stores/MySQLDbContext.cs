using Lys.MQConsumer.Service.Common;
using Lys.MQConsumer.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Lys.MQConsumer.Service.Stores
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TestUser>(b =>
            {
                b.Property(u => u.PhoneNumber).HasColumnName("Mobile");
                b.Property(u => u.PhoneNumberConfirmed).HasColumnName("MobileConfirmed");
            });

            builder.UseMySqlNamingStyle();
        }

        public DbSet<TestUser> Users { get; set; }
    }
}
