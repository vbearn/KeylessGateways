using Microsoft.EntityFrameworkCore;

namespace KeylessGateways.DoorEntrance.Data
{
    public class DoorEntranceDbContext : DbContext
    {
        public DoorEntranceDbContext(DbContextOptions<DoorEntranceDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserDoor> UserDoors { get; set; }
        public DbSet<DoorEntranceHistory> DoorEntrances { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
