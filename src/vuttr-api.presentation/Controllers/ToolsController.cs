using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vuttr_api.domain.dtos;
using vuttr_api.services.contracts;

namespace vuttr_api.presentation.controllers;

[ApiController]
[Route("api/tools")]
[Authorize]
public class ToolsController : ControllerBase
{
    private readonly IToolService _toolService;

    public ToolsController(IToolService toolService)
    {
        _toolService = toolService;
    }

    [HttpGet(Name = "GetAll")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolResponse>))]
    public async Task<IActionResult> GetAllToolsAsync()
    {
        return Ok(await _toolService.GetAllAsync());
    }

    [HttpGet("{id}", Name = "GetById")]
    [ProducesResponseType(200, Type = typeof(ToolResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToolByIdAsync(int id)
    {
        if (id <= 0) return BadRequest("Invalid tool id");
        ToolResponse? tool = await _toolService.GetByIdAsync(id);
        return tool is null ? NotFound($"Tool {id} could not be found") : Ok(tool);
    }

    [HttpGet("{tag}", Name = "GetByTag")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolResponse>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToolsByTagAsync(string? tag)
    {
        if (tag is null) return BadRequest("Invalid tool tag");
        IEnumerable<ToolResponse>? tools = await _toolService.GetByTagAsync(tag);
        return tools is null ? NotFound($"There is no tool with the tag '{tag}'") : Ok(tools);
    }

    [HttpPost(Name = "Create")]
    [ProducesResponseType(201, Type = typeof(ToolResponse))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateToolAsync([FromBody] CreateToolRequest newTool)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ToolResponse? added = await _toolService.RegisterAsync(newTool);
        return added is null
                ? BadRequest("Tool already exists")
                : CreatedAtAction("GetToolByIdAsync", added.Id, added);
    }

    [HttpDelete("{id}", Name = "Delete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteToolAsync(int id)
    {
        if (id <= 0) return BadRequest("Invalid tool id");

        ToolResponse? existing = await _toolService.GetByIdAsync(id);

        if (existing is null) return NotFound($"Tool {id} does not exist");

        bool? deleted = await _toolService.DeleteAsync(id);

        return deleted.HasValue && deleted.Value
            ? new NoContentResult()
            : BadRequest($"Tool {id} was found but failed to delete.");
    }
}
