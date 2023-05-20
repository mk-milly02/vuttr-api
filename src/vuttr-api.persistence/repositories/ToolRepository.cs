using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using vuttr_api.domain.entities;
using vuttr_api.domain.settings;

namespace vuttr_api.persistence.repositories;

public class ToolRepository : IToolRepository
{
    private readonly IConfiguration _configuration;
    private readonly MongoDbSettings? _settings;
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Tool> _collection;

    public ToolRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _settings = _configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

        _client = new MongoClient(_settings!.ConnectionString);
        _database = _client.GetDatabase(_settings.DatabaseName);
        _collection = _database.GetCollection<Tool>("Tools");
    }

    public async Task CreateAsync(Tool tool)
    {
        await _collection.InsertOneAsync(tool);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Tool? result = await _collection.FindOneAndDeleteAsync(tool => tool.Id.Equals(id));
        return result is not null;
    }

    public async Task<IEnumerable<Tool>?> RetrieveAllAsync()
    {
        IAsyncCursor<Tool> tools = await _collection.FindAsync(_ => true);
        return await tools.AnyAsync() ? tools.ToEnumerable() : null;
    }

    public async Task<IEnumerable<Tool>?> RetrieveAllByConditionAsync(Expression<Func<Tool, bool>> expression)
    {
        IAsyncCursor<Tool> tools = await _collection.FindAsync(expression);
        return !await tools.AnyAsync() ? null : tools.ToEnumerable();
    }

    public async Task<Tool?> RetrieveByConditionAsync(Expression<Func<Tool, bool>> expression)
    {
        IAsyncCursor<Tool> tools = await _collection.FindAsync(expression);

        return !await tools.AnyAsync() ? null : tools.FirstOrDefault();
    }
}