using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.persistence.repositories;
using vuttr_api.services.contracts;

namespace vuttr_api.services;

public class ToolService : IToolService
{
    private readonly IToolRepository _repository;
    private readonly IMapper _mapper;

    public ToolService(IToolRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ToolResponse>?> GetAllAsync()
    {
        List<ToolResponse> response = new();
        IEnumerable<Tool>? tools = await _repository.RetrieveAllAsync();

        if (tools is null) return null;

        foreach (Tool tool in tools)
        {
            ToolResponse t = _mapper.Map<ToolResponse>(tool);
            response.Add(t);
        }

        return response;
    }

    public async Task<ToolResponse?> GetByIdAsync(int id)
    {
        ToolResponse? response = new();
        Tool? tool = await _repository.RetrieveByConditionAsync(t => t.Id.Equals(id));

        return tool is null ? null : _mapper.Map<ToolResponse>(tool);
    }

    public async Task<IEnumerable<ToolResponse>?> GetByTagAsync(string tag)
    {
        List<ToolResponse>? response = new();
        IEnumerable<Tool>? tools = await _repository.RetrieveAllByConditionAsync(t => t.Tags!.Contains(tag));

        if (tools is null) return null;

        foreach (Tool tool in tools)
        {
            ToolResponse t = _mapper.Map<ToolResponse>(tool);
            response.Add(t);
        }

        return response;
    }

    public async Task<ToolResponse?> RegisterAsync(CreateToolRequest tool)
    {
        Tool? existing = await _repository.RetrieveByConditionAsync(t => t.Title!.Equals(tool.Title));

        if (existing is not null) return null;

        Tool nT = _mapper.Map<Tool>(tool);
        await _repository.CreateAsync(nT);

        Tool? added = await _repository.RetrieveByConditionAsync(t => t.Title!.Equals(tool.Title));

        return _mapper.Map<ToolResponse>(added);
    }
}