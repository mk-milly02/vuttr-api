using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using vuttr_api.domain.entities;

namespace vuttr_api.persistence.repositories;

public class ToolRepository : IToolRepository
{
    private readonly IConfiguration _configuration;
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Tool> _collection;

    public ToolRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new MongoClient(_configuration.GetConnectionString("DefaultConnection"));
        _database = _client.GetDatabase(_configuration.GetSection("Database").Value);
        _collection = _database.GetCollection<Tool>("Tools");
    }

    public async Task CreateAsync(Tool tool)
    {
        await _collection.InsertOneAsync(tool);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        DeleteResult result = await _collection.DeleteOneAsync(tool => tool.Id.Equals(id));
        return result.IsAcknowledged;
    }

    public async Task<IEnumerable<Tool>> RetrieveAllAsync()
    {
        IAsyncCursor<Tool> tools = await _collection.FindAsync(_ => true);
        return await tools.ToListAsync();
    }

    public async Task<IEnumerable<Tool>> RetrieveByConditionAsync(Expression<Func<Tool, bool>> expression)
    {
        IAsyncCursor<Tool> tools = await _collection.FindAsync(expression);
        return await tools.ToListAsync();
    }
}