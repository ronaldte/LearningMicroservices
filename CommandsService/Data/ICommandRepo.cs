using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepo
{
    bool SaveChanges();

    // Platforms
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
    bool ExternalPlatformExists(int exteranlPlatformId);

    // Commands
    IEnumerable<Command> GetAllCommandsForPlatform(int platformId);
    Command? GetCommandById(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}