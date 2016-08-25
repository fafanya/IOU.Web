using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fund.Models;

namespace Fund.Data
{
    public class ApplicationDbContext : IdentityDbContext<UUser>
    {
        public DbSet<UDebt> UDebts { get; set; }
        public DbSet<UBill> UBills { get; set; }
        public DbSet<UPayment> UPayments { get; set; }
        public DbSet<UEvent> UEvents { get; set; }
        public DbSet<UEventType> UEventTypes { get; set; }
        public DbSet<UGroup> UGroups { get; set; }
        public DbSet<UMember> UMembers { get; set; }
        public DbSet<UUser> UUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
