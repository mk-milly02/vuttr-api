using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;

namespace vuttr_api.infrastructure.mapping;

public class ReverseTagResolver : IValueResolver<Tool, ToolViewModel, List<string>?>
{
    public List<string>? Resolve(Tool source, ToolViewModel destination, List<string>? destMember, ResolutionContext context)
    {
        List<string> tags = new();

        foreach (Tag tag in source.Tags!)
        {
            tags.Add(tag.Name!);
        }

        return tags;
    }
}