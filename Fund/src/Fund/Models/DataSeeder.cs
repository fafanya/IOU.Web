using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Fund.Data;

namespace Fund.Models
{
    public class DataSeeder
    {
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
                                     Name = "Own"
                                 },

                                 new UEventType
                                 {
                                     Id = 2,
                                     Name = "Common"
                                 },

                                 new UEventType
                                 {
                                     Id = 3,
                                     Name = "Partly"
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