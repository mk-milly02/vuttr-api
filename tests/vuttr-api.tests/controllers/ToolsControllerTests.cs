using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.infrastructure.mapping;
using vuttr_api.presentation.controllers;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.controllers;

public class ToolsControllerTests
{
    private readonly Mock<IToolService> _mockToolService;
    private readonly IMapper _mapper;
    private ToolsController? _controller;

    public ToolsControllerTests()
    {
        _mockToolService = new Mock<IToolService>(MockBehavior.Loose);
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
    }

    private readonly List<ToolViewModel> tools = new()
    {
        new()
        {
            Id = 1,
            Title = "notion",
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

    [Fact]
    public async Task GetTools_ShouldReturnTools()
    {
        // Given
        _mockToolService!.Setup(x => x.GetToolsAsync()).ReturnsAsync(tools);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.GetTools();

        // Then
        _mockToolService.Verify(x => x.GetToolsAsync(), Times.Once);

        OkObjectResult responseObject = Assert.IsAssignableFrom<OkObjectResult>(response);
        IEnumerable<ToolViewModel> result = Assert.IsAssignableFrom<IEnumerable<ToolViewModel>>(responseObject.Value);
        Assert.True(result.Count() is 4);
    }

    [Fact]
    public void GetToolsByTag_ShouldReturnBadRequestWhenTagIsNull()
    {
        // Given
        string? tag = null;
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = _controller.GetToolsByTag(tag);

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Invalid tag.", responseObject.Value);
    }

    [Fact]
    public void GetToolsByTag_ShouldReturnToolsThatMatchTheTag()
    {
        // Given
        string tag = "web";
        _mockToolService!.Setup(x => x.GetToolsByTag(It.IsAny<string>())).Returns(tools.Where(x => x.Tags!.Contains(tag)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = _controller.GetToolsByTag(tag);

        // Then
        _mockToolService.Verify(x => x.GetToolsByTag(It.IsAny<string>()), Times.Once);

        OkObjectResult responseObject = Assert.IsAssignableFrom<OkObjectResult>(response);
        IEnumerable<ToolViewModel>? result = Assert.IsAssignableFrom<IEnumerable<ToolViewModel>?>(responseObject.Value);

        Assert.NotNull(result);
        Assert.True(result.Count() is 2);

        foreach (ToolViewModel tool in result)
        {
            Assert.NotNull(tool.Tags);
            Assert.Contains(tag, tool.Tags);
        }
    }

    [Fact]
    public async void RegisterTool_ShouldReturnBadRequestWhenModelIsInvalid()
    {
        // Given
        CreateToolRequest toolRequest = new()
        {
            Link = "qwerty",
            Description = "something"
        };

        _controller = new(_mockToolService.Object);
        _controller.ModelState.AddModelError("Title", "Title field is required");
        _controller.ModelState.AddModelError("Link", "Invalid url");
        _controller.ModelState.AddModelError("Tags", "A tool should have at least on tag");

        // When
        IActionResult response = await _controller.RegisterTool(toolRequest);

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.IsAssignableFrom<SerializableError>(responseObject.Value);
    }

    [Fact]
    public async Task RegisterTool_ShouldReturnBadRequestWhenToolAlreadyExists()
    {
        // Given
        CreateToolRequest toolRequest = new()
        {
            Title = "notion"
        };

        _mockToolService!.Setup(x => x.GetToolByTitle(It.IsAny<string>()))
            .Returns(tools.SingleOrDefault(x => x.Title!.Equals(toolRequest.Title)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.RegisterTool(toolRequest);

        // Then
        _mockToolService.Verify(x => x.GetToolByTitle(It.IsAny<string>()), Times.Once);
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Tool already exists.", responseObject.Value);
    }

    [Fact]
    public async Task RegisterTool_ShouldReturnBadRequestWhenRepositoryFailsToCreateTool()
    {
        // Given
        CreateToolRequest toolRequest = new()
        {
            Title = "hotel",
            Link = "https://github.com/typicode/hotel",
            Description = "Local app manager. Start apps within your browser, developer tool with local .localhost domain and https out of the box.",
            Tags = new() { "node", "organizing", "webapps", "domain", "developer", "https", "proxy" }
        };

        ToolViewModel? tool = null;

        _mockToolService!.Setup(x => x.GetToolByTitle(It.IsAny<string>()))
            .Returns(tools.SingleOrDefault(x => x.Title!.Equals(toolRequest.Title)));

        _mockToolService.Setup(x => x.CreateToolAsync(toolRequest))
            .ReturnsAsync(tool);

        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.RegisterTool(toolRequest);

        // Then
        _mockToolService.Verify(x => x.GetToolByTitle(It.IsAny<string>()), Times.Once);
        _mockToolService.Verify(x => x.CreateToolAsync(toolRequest), Times.Once);
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Repository failed to create tool.", responseObject.Value);
    }

    [Fact]
    public async void RegisterTool_ShouldReturnCreatedAtActionWhenToolIsCreated()
    {
        // Given
        CreateToolRequest toolRequest = new()
        {
            Title = "hotel",
            Link = "https://github.com/typicode/hotel",
            Description = "Local app manager. Start apps within your browser, developer tool with local .localhost domain and https out of the box.",
            Tags = new() { "node", "organizing", "webapps", "domain", "developer", "https", "proxy" }
        };

        _mockToolService!.Setup(x => x.GetToolByTitle(It.IsAny<string>()))
            .Returns(tools.SingleOrDefault(x => x.Title!.Equals(toolRequest.Title)));

        _mockToolService.Setup(x => x.CreateToolAsync(toolRequest))
            .Callback<CreateToolRequest>(c =>
            {
                Tool? x = _mapper.Map<Tool>(c);
                x.Id = 5;
                ToolViewModel? y = _mapper.Map<ToolViewModel>(x);
                tools.Add(y);
            })
            .ReturnsAsync(tools.Last());

        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.RegisterTool(toolRequest);

        // Then
        CreatedAtActionResult responseObject = Assert.IsAssignableFrom<CreatedAtActionResult>(response);
        Assert.True(tools.Count is 5);
    }


    [Fact]
    public void GetToolById_ShouldReturnBadRequestWhenIdIsInvalid()
    {
        // Given
        int id = 0;
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = _controller.GetToolById(id);

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Invalid id.", responseObject.Value);
    }

    [Fact]
    public void GetToolById_ShouldReturnNotFoundWhenNoToolMatchesTheId()
    {
        // Given
        int id = 8;
        _mockToolService!.Setup(x => x.GetToolById(It.IsAny<int>())).Returns(tools.SingleOrDefault(x => x.Id.Equals(id)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = _controller.GetToolById(id);

        // Then
        _mockToolService.Verify(x => x.GetToolById(It.IsAny<int>()), Times.Once);
        NotFoundObjectResult responseObject = Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        Assert.Equal($"There is no tool with the id: {id}.", responseObject.Value);
    }

    [Fact]
    public void GetToolById_ShouldReturnToolThatMatchesTheId()
    {
        // Given
        int id = 2;
        _mockToolService!.Setup(x => x.GetToolById(It.IsAny<int>())).Returns(tools.SingleOrDefault(x => x.Id.Equals(id)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = _controller.GetToolById(id);

        // Then
        _mockToolService.Verify(x => x.GetToolById(It.IsAny<int>()), Times.Once);
        OkObjectResult responseObject = Assert.IsAssignableFrom<OkObjectResult>(response);
        ToolViewModel? result = Assert.IsAssignableFrom<ToolViewModel?>(responseObject.Value);

        Assert.NotNull(result);
        Assert.True(result.Id is 2);
    }

    [Fact]
    public async Task UpdateTool_ShouldReturnBadRequestWhenIdIsInvalid()
    {
        // Given
        int id = 0;
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.UpdateTool(id, new());

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Invalid id.", responseObject.Value);
    }

    [Fact]
    public async void UpdateTool_ShouldReturnBadRequestWhenModelIsInvalid()
    {
        // Given
        UpdateToolRequest toolRequest = new()
        {
            Link = "qwerty",
            Description = "something"
        };

        _controller = new(_mockToolService.Object);
        _controller.ModelState.AddModelError("Title", "Title field is required");
        _controller.ModelState.AddModelError("Link", "Invalid url");
        _controller.ModelState.AddModelError("Tags", "A tool should have at least on tag");

        // When
        IActionResult response = await _controller.UpdateTool(2, toolRequest);

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.IsAssignableFrom<SerializableError>(responseObject.Value);
    }

    [Fact]
    public async Task UpdateTool_ShouldReturnNotFoundWhenToolDoesNotExist()
    {
        // Given
        int id = 6;

        _mockToolService!.Setup(x => x.GetToolById(id)).Returns(tools.SingleOrDefault(x => x.Id.Equals(id)));

        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.UpdateTool(id, new());

        // Then
        _mockToolService.Verify(x => x.GetToolById(id), Times.Once);
        NotFoundObjectResult responseObject = Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        Assert.Equal($"Tool with id: {id} does not exist.", responseObject.Value);
    }

    [Fact]
    public async void UpdateTool_ShouldReturnBadRequestWhenUpdateFails()
    {
        // Given
        int id = 1;

        UpdateToolRequest toolRequest = new()
        {
            Title = "notion",
            Link = "https://notion.com",
            Description = "All in one tool to organize teams and ideas. Write, plan, collaborate, and get organized. ",
            Tags = new() { "organization", "planning", "collaboration", "writing", "calendar" }
        };

        ToolViewModel? tool = null;

        _mockToolService!.Setup(x => x.GetToolById(id)).Returns(tools.SingleOrDefault(x => x.Id.Equals(id)));
        _mockToolService.Setup(x => x.UpdateToolAsync(id, toolRequest)).ReturnsAsync(tool);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.UpdateTool(id, toolRequest);

        // Then
        _mockToolService.Verify(x => x.GetToolById(id), Times.Once);
        _mockToolService.Verify(x => x.UpdateToolAsync(id, toolRequest), Times.Once);

        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Repository failed to update tool.", responseObject.Value);
    }

    [Fact]
    public async void UpdateTool_ShouldReturnOkWhenUpdateIsSuccessful()
    {
        // Given
        int id = 1;

        UpdateToolRequest toolRequest = new()
        {
            Link = "https://notion.com"
        };

        ToolViewModel? tool = new()
        {
            Id = 1,
            Title = "notion",
            Link = "https://notion.com",
            Description = "All in one tool to organize teams and ideas. Write, plan, collaborate, and get organized. ",
            Tags = new() { "organization", "planning", "collaboration", "writing", "calendar" }
        };

        _mockToolService!.Setup(x => x.GetToolById(id)).Returns(tools.SingleOrDefault(x => x.Id.Equals(id)));
        _mockToolService.Setup(x => x.UpdateToolAsync(id, toolRequest))
            .ReturnsAsync(tool);

        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.UpdateTool(id, toolRequest);

        // Then
        _mockToolService.Verify(x => x.GetToolById(id), Times.Once);
        _mockToolService.Verify(x => x.UpdateToolAsync(id, toolRequest), Times.Once);

        OkObjectResult responseObject = Assert.IsAssignableFrom<OkObjectResult>(response);
        ToolViewModel? result = Assert.IsAssignableFrom<ToolViewModel?>(responseObject.Value);
        Assert.NotNull(result);
        Assert.Equal(toolRequest.Link, result.Link);
        Assert.True(tools.Exists(x => x.Title is "notion"));
    }

    [Fact]
    public async Task DeleteTool_ShouldReturnBadRequestWhenIdIsInvalid()
    {
        // Given
        int id = 0;
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.DeleteTool(id);

        // Then
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Invalid id.", responseObject.Value);
    }

    [Fact]
    public async void DeleteTool_ShouldReturnNotFoundWhenToolDoesNotExist()
    {
        // Given
        int id = 100;
        bool? deleted = null;
        _mockToolService!.Setup(x => x.DeleteToolAsync(id)).ReturnsAsync(deleted);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.DeleteTool(id);

        // Then
        _mockToolService.Verify(x => x.DeleteToolAsync(id), Times.Once);

        NotFoundObjectResult responseObject = Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        Assert.Equal($"Tool with id: {id} does not exist.", responseObject.Value);
    }

    [Fact]
    public async void DeleteTool_ShouldReturnBadRequestWhenDeleteFails()
    {
        // Given
        int id = 3;
        _mockToolService!.Setup(x => x.DeleteToolAsync(id)).ReturnsAsync(!tools.Exists(x => x.Id.Equals(id)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.DeleteTool(id);

        // Then
        _mockToolService.Verify(x => x.DeleteToolAsync(id), Times.Once);

        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        Assert.Equal("Repository failed to delete tool.", responseObject.Value);
    }

    [Fact]
    public async void DeleteTool_ShouldReturnNoContentWhenDeleteIsSuccessful()
    {
        // Given
        int id = 2;

        _mockToolService!.Setup(x => x.DeleteToolAsync(id))
            .Callback<int>(x =>
            {
                ToolViewModel? toBeDeleted = tools.SingleOrDefault(t => t.Id.Equals(x));
                Assert.NotNull(toBeDeleted);
                tools.Remove(toBeDeleted);
            })
            .ReturnsAsync(true);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult response = await _controller.DeleteTool(id);

        // Then
        _mockToolService.Verify(x => x.DeleteToolAsync(id), Times.Once);

        Assert.IsAssignableFrom<NoContentResult>(response);
        Assert.False(tools.Exists(x => x.Title is "json-server"));
    }
}