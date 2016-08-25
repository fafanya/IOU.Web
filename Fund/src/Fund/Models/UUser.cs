using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Fund.Models
{
    // Add profile data for application users by adding properties to the UUser class
    public class UUser : IdentityUser
    {
        public List<UMember> UMembers { get; set; }
        public List<UGroup> UGroups { get; set; }
    }
}
