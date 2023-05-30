using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.persistence.contracts;
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

    public async Task<ToolViewModel?> CreateToolAsync(CreateToolRequest toolRequest)
    {
        Tool? toBeAdded = _mapper.Map<Tool>(toolRequest);
        Tool? added = await _repository.CreateAsync(toBeAdded);
        return added is null ? null : _mapper.Map<ToolViewModel>(added);
    }

    public async Task<bool?> DeleteToolAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public ToolViewModel? GetToolById(int id)
    {
        Tool? tool = _repository.RetrieveById(id);
        return tool is null ? null : _mapper.Map<ToolViewModel>(tool);
    }

    public async Task<IEnumerable<ToolViewModel>?> GetToolsAsync()
    {
        List<ToolViewModel> output = new();
        IEnumerable<Tool>? tools = await _repository.RetrieveAllAsync();
        if (tools is null) return null;

        foreach (Tool tool in tools)
        {
            ToolViewModel x = _mapper.Map<ToolViewModel>(tool);
            output.Add(x);
        }
        return output;
    }

    public IEnumerable<ToolViewModel>? GetToolsByTag(string tag)
    {
        List<ToolViewModel> output = new();
        IEnumerable<Tool>? tools = _repository.RetrieveByTag(tag);
        if (tools is null) return null;

        foreach (Tool tool in tools)
        {
            ToolViewModel x = _mapper.Map<ToolViewModel>(tool);
            output.Add(x);
        }
        return output;
    }

    public async Task<ToolViewModel?> UpdateToolAsync(int id, UpdateToolRequest toolRequest)
    {
        Tool? toBeUpdated = _mapper.Map<Tool>(toolRequest);
        toBeUpdated.Id = id;
        Tool? updated = await _repository.UpdateAsync(toBeUpdated);
        return updated is null ? null : _mapper.Map<ToolViewModel>(updated);
    }
}