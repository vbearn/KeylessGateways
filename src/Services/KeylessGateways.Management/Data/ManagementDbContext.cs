using Microsoft.EntityFrameworkCore;

namespace KeylessGateways.Management.Data
{
    public class ManagementDbContext : DbContext
    {
        public ManagementDbContext(DbContextOptions<ManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Door> Doors { get; set; }
        public DbSet<UserDoor> UserDoors { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
