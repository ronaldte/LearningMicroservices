using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>() ?? throw new ArgumentNullException("--> Grpc clinet service not found.");
        
        var platforms = grpcClient.ReturnAllPlatforms();

        var commandRepo = serviceScope.ServiceProvider.GetService<ICommandRepo>() ?? throw new ArgumentNullException("--> Command service not found");
        
        SeedData(commandRepo ,platforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        foreach(var platform in platforms)
        {
            if(!repo.ExternalPlatformExists(platform.ExternalID))
            {
                repo.CreatePlatform(platform);
            }
        }

        repo.SaveChanges();
    }
}