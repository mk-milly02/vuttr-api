using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;

namespace vuttr_api.infrastructure.mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Tool, ToolResponse>();
        CreateMap<CreateToolRequest, Tool>();
        CreateMap<UserForRegisteration, User>();
        CreateMap<User, UserResponse>();
    }
}