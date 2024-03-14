using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break; 
            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        // there is a problem with life time, where you cannot inject dependecy with lesser life time than
        // the service requesting the service, that why we have to manually request it.
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);
            if(repo is not null && !repo.ExternalPlatformExists(platform.ExternalID))
            {
                repo.CreatePlatform(platform);
                repo.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong with creating platform {ex.Message}");
        }

    }

    private EventType DetermineEvent(string notificationMessage)
    {
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        return (eventType?.Event) switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined,
        };
    }

}

enum EventType 
{
    PlatformPublished,
    Undetermined
}