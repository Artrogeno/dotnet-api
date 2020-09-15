using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
  //api/commands
  [Route("api/commands")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommanderRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommanderRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    // Get api/commands
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
    {
      var commandItems = _repository.GetAllCommands();
      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
    }

    //Get api/commands/{id}
    [HttpGet("{id}", Name = "GetCommandById")]
    public ActionResult<CommandReadDto> GetCommandById(int id)
    {
      var commandItem = _repository.GetCommandById(id);

      if (commandItem != null)
      {
        return Ok(_mapper.Map<CommandReadDto>(commandItem));
      }
      return NotFound();
    }

    //Post api/commands
    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
    {
      var commandModal = _mapper.Map<Command>(commandCreateDto);
      _repository.CreateCommand(commandModal);
      _repository.SaveChanges();

      var commandReadDto = _mapper.Map<CommandReadDto>(commandModal);

      return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto);
      // return Ok(commandReadDto);
    }

    //Put api/commands/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
    {
      var commandModalFromRepo = _repository.GetCommandById(id);
      if (commandModalFromRepo == null)
      {
        return NotFound();
      }
      _mapper.Map(commandUpdateDto, commandModalFromRepo);
      _repository.UpdateCommand(commandModalFromRepo);
      _repository.SaveChanges();
      return NoContent();

    }

    //Patch api/commands/{id}
    [HttpPatch("{id}")]
    public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
    {
      var commandModalFromRepo = _repository.GetCommandById(id);
      if (commandModalFromRepo == null)
      {
        return NotFound();
      }

      var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModalFromRepo);
      patchDoc.ApplyTo(commandToPatch,
                       ModelState);
      if (!TryValidateModel(commandToPatch))
      {
        return ValidationProblem(ModelState);
      }

      _mapper.Map(commandToPatch, commandModalFromRepo);
      _repository.UpdateCommand(commandModalFromRepo);
      _repository.SaveChanges();
      return NoContent();
    }

    //Delete api/commands/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteCommand(int id)
    {
      var commandModalFromRepo = _repository.GetCommandById(id);
      if (commandModalFromRepo == null)
      {
        return NotFound();
      }
      _repository.DeleteCommand(commandModalFromRepo);
      _repository.SaveChanges();

      return NoContent();
    }
  }
}
