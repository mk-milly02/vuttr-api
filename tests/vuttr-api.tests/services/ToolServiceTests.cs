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

    [Fact]
    public async Task RegisterAsync_ShouldNotRegisterTool()
    {
        Tool tool = new()
        {
            Id = 1,
            Title = "Resume Builder",
            Link = "https://resume.io",
            Description = "All in one tool to build and organize resumes. ",
            Tags = new() { "resume", "cv", "writing" }
        };

        _mockToolRepository!.Setup(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                            .ReturnsAsync(tool);

        CreateToolRequest newTool = new()
        {
            Title = "Resume Builder",
            Link = "https://resume.io",
            Description = "All in one tool to build and organize resumes. ",
            Tags = new() { "resume", "cv", "writing" }
        };

        ToolResponse? actual = await _toolService.RegisterAsync(newTool);

        Assert.Null(actual);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterTool()
    {
        Tool? first = null;

        Tool? second = new()
        {
            Id = 1,
            Title = "Resume Builder",
            Link = "https://resume.io",
            Description = "All in one tool to build and organize resumes. ",
            Tags = new() { "resume", "cv", "writing" }
        };

        _mockToolRepository!.Setup(mtr => mtr.CreateAsync(It.IsAny<Tool>())).Returns(Task.CompletedTask);
        _mockToolRepository.SetupSequence(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                           .ReturnsAsync(first)
                           .ReturnsAsync(second);

        CreateToolRequest createTool = new()
        {
            Title = "Resume Builder",
            Link = "https://resume.io",
            Description = "All in one tool to build and organize resumes. ",
            Tags = new() { "resume", "cv", "writing" }
        };

        ToolResponse? expected = _mapper.Map<ToolResponse>(second);
        ToolResponse? actual = await _toolService.RegisterAsync(createTool);

        _mockToolRepository.Verify(mtr => mtr.CreateAsync(It.IsAny<Tool>()), Times.Once);
        _mockToolRepository.Verify(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()), Times.Exactly(2));

        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Link, actual.Link);
        Assert.Equal(expected.Title, actual.Title);

        foreach (string tag in actual.Tags!)
        {
            Assert.Contains(tag, expected.Tags!);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNull()
    {
        List<Tool>? tools = null;

        _mockToolRepository!.Setup(mtr => mtr.RetrieveAllAsync()).ReturnsAsync(tools);

        IEnumerable<ToolResponse>? response = await _toolService.GetAllAsync();

        _mockToolRepository.Verify(mtr => mtr.RetrieveAllAsync(), Times.Once);
        Assert.Null(response);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTools()
    {
        List<Tool>? tools = new()
        {
            new()
            {
                Id = 1,
                Title = "Notion",
                Link = "https://notion.so",
                Description = "All in one tool to organize teams and ideas. Write, plan, collaborate, and get organized. ",
                Tags = new() { "organization", "planning", "collaboration", "writing", "calendar" }
            },
            new()
            {
                Id = 2,
                Title = "json-server",
                Link = "https://github.com/typicode/json-server",
                Description = "Fake REST API based on a json schema. Useful for mocking and creating APIs for front-end devs to consume in coding challenges. ",
                Tags = new() { "api", "json", "schema", "node", "github", "rest" }
            },
            new()
            {
                Id = 3,
                Title = "fastify",
                Link = "https://www.fastify.io/",
                Description = "Extremely fast and simple, low-overhead web framework for NodeJS. Supports HTTP2.",
                Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
            },
            new()
            {
                Id = 4,
                Title = "user-jwt",
                Link = "https://www.user-jwt.ai/",
                Description = "something",
                Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
            }
        };

        _mockToolRepository!.Setup(mtr => mtr.RetrieveAllAsync()).ReturnsAsync(tools);

        IEnumerable<ToolResponse>? actual = await _toolService.GetAllAsync();

        _mockToolRepository.Verify(mtr => mtr.RetrieveAllAsync(), Times.Once);
        Assert.NotNull(actual);
        Assert.True(actual!.Count() is 4);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        Tool? tool = null;

        _mockToolRepository!.Setup(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                            .ReturnsAsync(tool);

        ToolResponse? actual = await _toolService.GetByIdAsync(12);

        _mockToolRepository.Verify(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()), Times.Once);
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTool()
    {
        Tool? tool = new()
        {
            Id = 1,
            Title = "Resume Builder",
            Link = "https://resume.io",
            Description = "All in one tool to build and organize resumes. ",
            Tags = new() { "resume", "cv", "writing" }
        };

        _mockToolRepository!.Setup(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                            .ReturnsAsync(tool);

        ToolResponse? actual = await _toolService.GetByIdAsync(1);
        ToolResponse? expected = _mapper.Map<ToolResponse>(tool);

        _mockToolRepository.Verify(mtr => mtr.RetrieveByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()), Times.Once);
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public async Task GetByTagAsync_ShouldReturnNull()
    {
        IEnumerable<Tool>? tool = null;

        _mockToolRepository!.Setup(mtr => mtr.RetrieveAllByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                            .ReturnsAsync(tool);

        IEnumerable<ToolResponse>? actual = await _toolService.GetByTagAsync("tag");

        _mockToolRepository.Verify(mtr => mtr.RetrieveAllByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()), Times.Once);
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetByTagAsync_ShouldReturnTools()
    {
        List<Tool>? tools = new()
        {
            new()
            {
                Id = 3,
                Title = "fastify",
                Link = "https://www.fastify.io/",
                Description = "Extremely fast and simple, low-overhead web framework for NodeJS. Supports HTTP2.",
                Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
            },
            new()
            {
                Id = 4,
                Title = "user-jwt",
                Link = "https://www.user-jwt.ai/",
                Description = "something",
                Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
            }
        };

        _mockToolRepository!.Setup(mtr => mtr.RetrieveAllByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()))
                            .ReturnsAsync(tools);

        IEnumerable<ToolResponse>? actual = await _toolService.GetByTagAsync("web");

        _mockToolRepository.Verify(mtr => mtr.RetrieveAllByConditionAsync(It.IsAny<Expression<Func<Tool, bool>>>()), Times.Once);
        Assert.NotNull(actual);
        Assert.True(actual.Count() is 2);

        foreach (ToolResponse tool in actual)
        {
            Assert.Contains("web", tool.Tags!);
        }
    }

    [Fact]
    public async Task DeleteAsync()
    {
        _mockToolRepository!.Setup(mtr => mtr.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
        bool? response = await _toolService.DeleteAsync(2);
        _mockToolRepository!.Verify(mtr => mtr.DeleteAsync(It.IsAny<int>()), Times.Once);
    }
}