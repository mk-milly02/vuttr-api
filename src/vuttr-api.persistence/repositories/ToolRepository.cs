using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using vuttr_api.domain.entities;
using vuttr_api.persistence.contracts;

namespace vuttr_api.persistence.repositories;

public class ToolRepository : IToolRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Tool> _tools;

    public ToolRepository(ApplicationDbContext context)
    {
        _context = context;
        _tools = _context.Set<Tool>();
    }

    public async Task<Tool?> CreateAsync(Tool tool)
    {
        EntityEntry<Tool> added = await _tools.AddAsync(tool);
        return await _context.SaveChangesAsync() > 0 ? added.Entity : null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Tool? existing = await _tools.FindAsync(id);

        if (existing is null) return null;
        _tools.Remove(existing);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Tool>?> RetrieveAllAsync()
    {
        return await _tools.Include(x => x.Tags).AsNoTracking().ToListAsync();
    }

    public Tool? RetrieveById(int id)
    {
        return _tools.Include(x => x.Tags).AsNoTracking().SingleOrDefault(x => x.Id.Equals(id));
    }

    public IEnumerable<Tool>? RetrieveByTag(string tag)
    {
        return _tools.Include(x => x.Tags)
                     .AsNoTracking()
                     .Where(x => x.Tags!.Any(x => x.Name!.Equals(tag)))
                     .AsEnumerable();
    }

    public Tool? RetrieveByTitle(string title)
    {
        return _tools.Include(x => x.Tags).AsNoTracking().SingleOrDefault(x => x.Title!.Equals(title));
    }

    public async Task<Tool?> UpdateAsync(Tool tool)
    {
        EntityEntry<Tool> updated = _tools.Update(tool);
        return await _context.SaveChangesAsync() > 0 ? updated.Entity : null;
    }
}