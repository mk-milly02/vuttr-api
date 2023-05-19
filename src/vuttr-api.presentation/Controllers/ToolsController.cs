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
    public async Task<IEnumerable<ToolResponse>> GetAllToolsAsync()
    {
        return await _toolService.GetAllAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(ToolResponse))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetToolByIdAsync(int? id)
    {
        if (id is null) return BadRequest();
        ToolResponse? tool = await _toolService.GetByIdAsync(id);
        return Ok(tool);
    }

    [HttpGet("{tag}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ToolResponse>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetToolsByTagAsync(string? tag)
    {
        if (tag is null) return BadRequest();
        IEnumerable<ToolResponse>? tools = await _toolService.GetByTagAsync(tag);
        return Ok(tools);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(ToolResponse))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateToolAsync([FromBody] CreateToolRequest newTool)
    {
        if (newTool is null) return BadRequest();
        ToolResponse? added = await _toolService.RegisterAsync(newTool);
        return added == null ? BadRequest("Repository failed to create customer.") : CreatedAtAction("GetToolByIdAsync", added.Id, added);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return BadRequest();
        ToolResponse? existing = await _toolService.GetByIdAsync(id);

        if (existing == null) return NotFound();

        bool? deleted = await _toolService.DeleteAsync(id);

        return deleted.HasValue && deleted.Value
            ? new NoContentResult()
            : BadRequest($"Tool {id} was found but failed to delete.");
    }
}
