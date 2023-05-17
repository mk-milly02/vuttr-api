using vuttr_api.domain.dtos;

namespace vuttr_api.services.contracts;

public interface IToolService
{
    Task<IEnumerable<ToolResponse>> GetAllAsync();
    Task<IEnumerable<ToolResponse>?> GetByTagAsync(string tag);
    Task<ToolResponse> RegisterAsync(CreateToolRequest tool);
    Task<bool> DeleteAsync(int id);
}