using Lys.Service.Common;
using Lys.Service.Models.Passport;
using Microsoft.EntityFrameworkCore;

namespace Lys.Service.Stores
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
