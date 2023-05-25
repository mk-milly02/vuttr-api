using System.Linq.Expressions;
using AutoMapper;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.infrastructure.mapping;
using vuttr_api.persistence.repositories;
using vuttr_api.services;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.services;

public class ToolServiceTests
{
    private readonly IToolService _toolService;
    private readonly Mock<IToolRepository>? _mockToolRepository;
    private readonly IMapper _mapper;

    public ToolServiceTests()
    {
        _mockToolRepository = new Mock<IToolRepository>(MockBehavior.Loose);
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _toolService = new ToolService(_mockToolRepository.Object, _mapper);
    }
}