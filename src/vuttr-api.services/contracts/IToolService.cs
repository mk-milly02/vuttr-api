using vuttr_api.domain.dtos;

namespace vuttr_api.services.contracts;

public interface IToolService
{
    Task<ToolViewModel?> CreateToolAsync(CreateToolRequest toolRequest);
    Task<IEnumerable<ToolViewModel>?> GetToolsAsync();
    ToolViewModel? GetToolById(int id);
    ToolViewModel? GetToolByTitle(string title);
    IEnumerable<ToolViewModel>? GetToolsByTag(string tag);
    Task<ToolViewModel?> UpdateToolAsync(int id, UpdateToolRequest toolRequest);
    Task<bool?> DeleteToolAsync(int id);
}