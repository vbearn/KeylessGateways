using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KeylessGateways.Identity.Data
{
    public class UserRole : IdentityUserRole<Guid> {

        public User User { get; set; }

        public Role Role { get; set; }

    }
}
