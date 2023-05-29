using vuttr_api.domain.entities;

namespace vuttr_api.persistence.contracts;

public interface IToolRepository
{
    Task<Tool?> CreateAsync(Tool tool);
    Task<IEnumerable<Tool>?> RetrieveAllAsync();
    Tool? RetrieveById(int id);
    IEnumerable<Tool>? RetrieveByTag(string tag);
    Task<Tool?> UpdateAsync(Tool tool);
    Task<bool?> DeleteAsync(int id);
}