using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KeylessGateways.Identity.Data
{
    public class Role : IdentityRole<Guid>  {

        public IList<UserRole> Users { get; set; } = new List<UserRole>();

    }
}
