using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controller;

[ApiController]
[Route("api/c/platforms/{platformId:int}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        if(!_repository.PlatformExists(platformId))
        {
            return NotFound($"Platform with ID {platformId} not found.");
        }

        var commands = _repository.GetAllCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        if(!_repository.PlatformExists(platformId))
        {
            return NotFound($"Platform with ID {platformId} not found.");
        }

        var command = _repository.GetCommandById(platformId, commandId);
        if(command is null)
        {
            return NotFound($"Command with ID {commandId} not found.");
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        if(!_repository.PlatformExists(platformId))
        {
            return NotFound($"Platform with ID {platformId} not found.");
        }

        var command = _mapper.Map<Command>(commandCreateDto);
        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);
        
        return CreatedAtAction("GetCommandForPlatform", new {platformId, commandId = command.Id}, commandReadDto);
    }
}