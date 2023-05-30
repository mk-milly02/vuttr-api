using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;

namespace vuttr_api.infrastructure.mapping;

public class TagResolver : IValueResolver<CreateToolRequest, Tool, IList<Tag>?>
{
    public IList<Tag> Resolve(CreateToolRequest source, Tool destination, IList<Tag>? destMember, ResolutionContext context)
    {
        List<Tag>? tags = new();

        foreach (string tag in source.Tags!)
        {
            Tag x = new() { Name = tag };
            tags.Add(x);
        }
        return tags;
    }
}