using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var platform = _context.Platforms.Where(p => p.Id == platformId).FirstOrDefault();
        platform?.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        _context.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int exteranlPlatformId)
    {
        return _context.Platforms.Any(p => p.ExternalID == exteranlPlatformId);
    }

    public IEnumerable<Command> GetAllCommandsForPlatform(int platformId)
    {
        return _context.Commands.Where(c => c.PlatformId == platformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Command? GetCommandById(int platformId, int commandId)
    {
        return _context.Commands.Where(c => c.PlatformId == platformId).FirstOrDefault(c => c.Id == commandId);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges() => _context.SaveChanges() > 0;
}