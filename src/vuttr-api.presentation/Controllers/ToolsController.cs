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

    [HttpGet] // api/tools
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolViewModel>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTools()
    {
        IEnumerable<ToolViewModel>? tools = await _toolService.GetToolsAsync();
        return tools is null ? NotFound("No tools available.") : Ok(tools);
    }

    [HttpGet("search")] // api/tools/search?tag=json
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetToolsByTag([FromQuery] string? tag)
    {
        if (tag is null) return BadRequest("Invalid tag.");

        IEnumerable<ToolViewModel>? tools = _toolService.GetToolsByTag(tag);
        return tools is null ? NotFound($"There is no tool with the tag '{tag}'.") : Ok(tools);
    }

    [HttpGet("{id}")] // api/tools/1
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult GetToolById(int id)
    {
        if (id <= 0) return BadRequest("Invalid id.");

        ToolViewModel? tool = _toolService.GetToolById(id);
        return tool is null ? NotFound($"There is no tool with the id: {id}.") : Ok(tool);
    }

    [HttpPost] // api/tools
    [ProducesResponseType(400)]
    [ProducesResponseType(201)]
    public async Task<IActionResult> RegisterTool([FromBody] CreateToolRequest toolRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ToolViewModel? existing = _toolService.GetToolByTitle(toolRequest.Title!);

        if (existing is not null) return BadRequest("Tool already exists.");

        ToolViewModel? added = await _toolService.CreateToolAsync(toolRequest);

        return added is null
            ? BadRequest("Repository failed to create tool.")
            : CreatedAtAction("GetToolById", added.Id, added);
    }

    [HttpPut("edit/{id}")] // api/tools/edit/1
    [ProducesResponseType(200, Type = typeof(ToolViewModel))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTool(int id, [FromBody] UpdateToolRequest toolRequest)
    {
        if (id <= 0) return BadRequest("Invalid id.");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        ToolViewModel? existing = _toolService.GetToolById(id);

        if (existing is null) return NotFound($"Tool with id: {id} does not exist.");

        ToolViewModel? updated = await _toolService.UpdateToolAsync(id, toolRequest);
        return updated is null ? BadRequest("Repository failed to update tool.") : Ok(updated);
    }

    [HttpDelete(":{id}")] // api/tools/:1
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteTool(int id)
    {
        if (id <= 0) return BadRequest("Invalid id.");

        bool? deleted = await _toolService.DeleteToolAsync(id);

        if (deleted is null) return NotFound($"Tool with id: {id} does not exist.");

        return deleted.Value ? NoContent() : BadRequest("Repository failed to delete tool.");
    }
}