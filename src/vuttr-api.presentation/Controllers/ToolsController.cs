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

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolResponse>))]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetAllToolsAsync()
    {
        IEnumerable<ToolResponse>? tools = await _toolService.GetAllAsync();
        return tools is null ? NoContent() : Ok(tools);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(ToolResponse))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToolByIdAsync(Guid id)
    {
        ToolResponse? tool = await _toolService.GetByIdAsync(id);
        return tool is null ? NotFound($"Tool {id} could not be found") : Ok(tool);
    }

    [HttpGet("{tag}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolResponse>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToolsByTagAsync(string? tag)
    {
        if (tag is null) return BadRequest("Invalid tool tag");
        IEnumerable<ToolResponse>? tools = await _toolService.GetByTagAsync(tag);
        return tools is null ? NotFound($"There is no tool with the tag '{tag}'") : Ok(tools);
    }

    [HttpGet("{title}")]
    [ProducesResponseType(200, Type = typeof(ToolResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToolByTitleAsync(string? title)
    {
        if (title is null) return BadRequest("Invalid tool title");
        ToolResponse? tool = await _toolService.GetByTitleAsync(title);
        return tool is null ? NotFound($"There is no tool with the title '{title}'") : Ok(tool);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateToolAsync([FromBody] CreateToolRequest newTool)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        bool added = await _toolService.RegisterAsync(newTool);
        return added
                ? CreatedAtAction(nameof(GetToolByTitleAsync), newTool.Title)
                : BadRequest("Tool already exists");
    }

    [HttpDelete(":{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteToolAsync(Guid id)
    {
        ToolResponse? existing = await _toolService.GetByIdAsync(id);

        if (existing is null) return NotFound($"Tool {id} does not exist");

        bool? deleted = await _toolService.DeleteAsync(id);

        return deleted.HasValue && deleted.Value
            ? new NoContentResult()
            : BadRequest($"Tool {id} was found but failed to delete.");
    }
}
