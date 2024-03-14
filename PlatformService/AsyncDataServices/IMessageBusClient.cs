using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices;

public interface IMessageBusClient
{
    void PubilshNewPlatform(PlatformPublishedDto platformPublishedDto);
}