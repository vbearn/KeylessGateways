using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KeylessGateways.Identity.Data
{
    public class KGIdentityDbContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public KGIdentityDbContext(DbContextOptions<KGIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
