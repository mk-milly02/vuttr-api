using System.Linq.Expressions;
using vuttr_api.domain.entities;

namespace vuttr_api.persistence.repositories;

public interface IToolRepository
{
    Task<IEnumerable<Tool>> RetrieveAllAsync();
    Task<IEnumerable<Tool>> RetrieveByConditionAsync(Expression<Func<Tool, bool>> expression);
    Task CreateAsync(Tool tool);
    Task<bool> DeleteAsync(int id);
}