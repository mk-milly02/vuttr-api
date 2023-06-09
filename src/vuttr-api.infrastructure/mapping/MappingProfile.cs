using AutoMapper;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;

namespace vuttr_api.infrastructure.mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateToolRequest, Tool>()
            .ForMember(tool => tool.Tags, opt => opt.MapFrom<TagResolver>());
        CreateMap<Tool, ToolViewModel>()
            .ForMember(tool => tool.Tags, opt => opt.MapFrom<ReverseTagResolver>());
        CreateMap<UpdateToolRequest, Tool>()
            .ForMember(tool => tool.Tags, opt => opt.MapFrom<UpdateTagResolver>());
        CreateMap<UserForRegistration, ApplicationUser>();
        CreateMap<ApplicationUser, UserViewModel>();
    }
}