using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool IsProduction)
    {
        using(var serviceScope = app.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
            ArgumentNullException.ThrowIfNull(dbContext);
            
            SeedData(dbContext, IsProduction);
        }
    }

    private static void SeedData(AppDbContext context, bool IsProduction)
    {
        if(IsProduction)
        {
            try
            {
                context.Database.Migrate();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Migrations failed {ex}]");
            }
        }

        if(!context.Platforms.Any())
        {
            Console.WriteLine("[Seeding data]");
            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"}
            );

            context.SaveChanges();
        }
    }
}