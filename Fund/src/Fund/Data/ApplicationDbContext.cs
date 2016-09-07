using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Fund.Models;

namespace Fund.Data
{
    public class ApplicationDbContext : IdentityDbContext<UUser>
    {
        public DbSet<UDebt> UDebts { get; set; }
        public DbSet<UBill> UBills { get; set; }
        public DbSet<UUser> UUsers { get; set; }
        public DbSet<UGroup> UGroups { get; set; }
        public DbSet<UEvent> UEvents { get; set; }
        public DbSet<UMember> UMembers { get; set; }
        public DbSet<UPayment> UPayments { get; set; }
        public DbSet<UEventType> UEventTypes { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<ApplicationDbContext>())
            {
                try
                {
                    if (context.UEventTypes != null)
                    {
                        if (!context.UEventTypes.Any())
                        {
                            context.UEventTypes.AddRange(
                                 new UEventType
                                 {
                                     Id = 1,
                                     Name = "Личный"
                                 },

                                 new UEventType
                                 {
                                     Id = 2,
                                     Name = "Общий"
                                 },

                                 new UEventType
                                 {
                                     Id = 3,
                                     Name = "Полуобщий"
                                 }
                            );
                            context.SaveChanges();
                        }
                    }
                }
                catch { }
            }
        }
    }
}
