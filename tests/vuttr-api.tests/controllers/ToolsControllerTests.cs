using Microsoft.AspNetCore.Mvc;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.presentation.controllers;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.controllers;

public class ToolsControllerTests
{
    private ToolsController? _controller;
    private readonly Mock<IToolService>? _mockToolService;

    public ToolsControllerTests()
    {
        _mockToolService = new(MockBehavior.Loose);
    }

    [Fact]
    public async Task GetAllToolsAsync_ShouldReturnNull()
    {
        // Given
        IEnumerable<ToolResponse>? tools = null;
        _mockToolService!.Setup(ts => ts.GetAllAsync()).ReturnsAsync(tools);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetAllToolsAsync();

        // Then
        _mockToolService.Verify(ts => ts.GetAllAsync(), Times.Once);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        Assert.Null(response.Value);
    }

    [Fact]
    public async Task GetAllToolsAsync_ShouldReturnTools()
    {
        // Given
        List<ToolResponse>? tools = new()
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
        _mockToolService!.Setup(ts => ts.GetAllAsync()).ReturnsAsync(tools);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetAllToolsAsync();

        // Then
        _mockToolService.Verify(ts => ts.GetAllAsync(), Times.Once);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        Assert.NotNull(response.Value);
        IEnumerable<ToolResponse>? allTools = Assert.IsAssignableFrom<IEnumerable<ToolResponse>?>(response.Value);
        Assert.Equal(2, allTools!.Count());
    }

    [Fact]
    public async Task GetToolByIdAsync_ShouldReturnBadRequest()
    {
        // Given
        _controller = new(_mockToolService!.Object);

        // When
        IActionResult result = await _controller.GetToolByIdAsync(-2);

        // Then
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal("Invalid tool id", response.Value);
    }

    [Fact]
    public async Task GetToolByIdAsync_ShouldReturnNotFound()
    {
        // Given
        int toolId = 5;
        List<ToolResponse>? tools = new()
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
        _mockToolService!.Setup(ts => ts.GetByIdAsync(toolId)).ReturnsAsync(tools.SingleOrDefault(x => x.Id.Equals(toolId)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetToolByIdAsync(toolId);

        // Then
        _mockToolService.Verify(ts => ts.GetByIdAsync(toolId), Times.Once);
        Assert.NotNull(result);
        NotFoundObjectResult response = Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        Assert.Equal($"Tool {toolId} could not be found", response.Value);
    }

    [Fact]
    public async Task GetToolByIdAsync_ShouldReturnOk()
    {
        // Given
        int toolId = 4;
        List<ToolResponse>? tools = new()
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
        _mockToolService!.Setup(ts => ts.GetByIdAsync(toolId)).ReturnsAsync(tools.SingleOrDefault(x => x.Id.Equals(toolId)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetToolByIdAsync(toolId);

        // Then
        _mockToolService.Verify(ts => ts.GetByIdAsync(toolId), Times.Once);
        Assert.NotNull(result);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        ToolResponse responseObject = Assert.IsAssignableFrom<ToolResponse>(response.Value);
        Assert.NotNull(responseObject);
        Assert.Equal(responseObject.Id, tools[1].Id);
        Assert.Equal(responseObject.Title, tools[1].Title);
        Assert.Equal(responseObject.Link, tools[1].Link);
        Assert.Equal(responseObject.Description, tools[1].Description);

        foreach (string tag in responseObject.Tags!)
        {
            Assert.Contains(tag, tools[1].Tags!);
        }
    }

    [Fact]
    public async Task GetToolByTagAsync_ShouldReturnBadRequest()
    {
        // Given
        _controller = new(_mockToolService!.Object);

        // When
        IActionResult result = await _controller.GetToolsByTagAsync(null);

        // Then
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal("Invalid tool tag", response.Value);
    }

    [Fact]
    public async Task GetToolByTagAsync_ShouldReturnNotFound()
    {
        // Given
        string tag = "tag";
        IEnumerable<ToolResponse>? tools = null;
        _mockToolService!.Setup(ts => ts.GetByTagAsync(tag)).ReturnsAsync(tools);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetToolsByTagAsync(tag);

        // Then
        _mockToolService.Verify(ts => ts.GetByTagAsync(tag), Times.Once);
        Assert.NotNull(result);
        NotFoundObjectResult response = Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        Assert.Equal($"There is no tool with the tag '{tag}'", response.Value);
    }

    [Fact]
    public async Task GetToolByTagAsync_ShouldReturnOk()
    {
        // Given
        string tag = "localhost";
        List<ToolResponse>? tools = new()
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
        _mockToolService!.Setup(ts => ts.GetByTagAsync(tag)).ReturnsAsync(tools.Where(t => t.Tags!.Contains(tag)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.GetToolsByTagAsync(tag);

        // Then
        _mockToolService.Verify(ts => ts.GetByTagAsync(tag), Times.Once);
        Assert.NotNull(result);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        IEnumerable<ToolResponse>? responseObject = Assert.IsAssignableFrom<IEnumerable<ToolResponse>?>(response.Value);
        Assert.NotNull(responseObject);
        Assert.Equal(2, responseObject.Count());
    }

    [Fact]
    public async Task CreateToolAsync_ShouldReturnBadRequestWhenModelIsInvalid()
    {
        // Given
        CreateToolRequest request = new()
        {
            Link = "https://something.name.com",
            Description = "Something",
            Tags = new() { "something" }
        };
        _controller = new(_mockToolService!.Object);
        _controller.ModelState.AddModelError("Title", "Title is required");

        // When
        IActionResult result = await _controller.CreateToolAsync(request);

        // Then
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        SerializableError error = Assert.IsType<SerializableError>(response.Value);
        KeyValuePair<string, object> expectedError = new("Title", new[] { "Title is required" });
        Assert.Equal(error.First().Key, expectedError.Key);
        Assert.Equal(error.First().Value, expectedError.Value);

    }

    [Fact]
    public async Task CreateToolAsync_ShouldReturnBadRequestWhenToolAlreadyExists()
    {
        // Given
        CreateToolRequest createTool = new()
        {
            Title = "fastify",
            Link = "https://www.fastify.io/",
            Description = "Extremely fast and simple, low-overhead web framework for NodeJS. Supports HTTP2.",
            Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
        };

        ToolResponse? tool = null;

        _mockToolService!.Setup(ts => ts.RegisterAsync(createTool)).ReturnsAsync(tool);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.CreateToolAsync(createTool);

        // Then
        _mockToolService.Verify(ts => ts.RegisterAsync(createTool), Times.Once);
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal($"Tool already exists", response.Value);
    }

    [Fact]
    public async Task CreateToolAsync_ShouldReturnOk()
    {
        // Given
        CreateToolRequest createTool = new()
        {
            Title = "fastify",
            Link = "https://www.fastify.io/",
            Description = "Extremely fast and simple, low-overhead web framework for NodeJS. Supports HTTP2.",
            Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
        };

        ToolResponse? tool = new()
        {
            Id = 7,
            Title = "fastify",
            Link = "https://www.fastify.io/",
            Description = "Extremely fast and simple, low-overhead web framework for NodeJS. Supports HTTP2.",
            Tags = new() { "web", "framework", "node", "http2", "https", "localhost" }
        };

        _mockToolService!.Setup(ts => ts.RegisterAsync(createTool)).ReturnsAsync(tool);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.CreateToolAsync(createTool);

        // Then
        _mockToolService.Verify(ts => ts.RegisterAsync(createTool), Times.Once);
        Assert.NotNull(result);
        CreatedAtActionResult response = Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        ToolResponse? responseObject = Assert.IsAssignableFrom<ToolResponse?>(response.Value);
        Assert.NotNull(responseObject);
        Assert.Equal(tool.Title, responseObject.Title);
    }

    [Fact]
    public async Task DeleteToolAsync_ShouldReturnBadRequestForInvalidId()
    {
        // Given
        _controller = new(_mockToolService!.Object);

        // When
        IActionResult result = await _controller.DeleteToolAsync(-2);

        // Then
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Invalid tool id");
    }

    [Fact]
    public async void DeleteToolAsync_ShouldReturnNotFound()
    {
        // Given
        List<ToolResponse>? tools = new()
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
        int toolId = 5;
        _mockToolService!.Setup(ts => ts.GetByIdAsync(toolId)).ReturnsAsync(tools.SingleOrDefault(x => x.Id.Equals(toolId)));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.DeleteToolAsync(toolId);

        // Then
        _mockToolService.Verify(ts => ts.GetByIdAsync(toolId), Times.Once);
        Assert.NotNull(result);
        NotFoundObjectResult response = Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        Assert.Equal(response.Value, $"Tool {toolId} does not exist");
    }

    [Fact]
    public async void DeleteToolAsync_ShouldDeleteTool()
    {
        // Given
        List<ToolResponse>? tools = new()
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

        List<ToolResponse>? toolsAfterDeletion = new()
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
            }
        };

        int toolId = 4;
        _mockToolService!.Setup(ts => ts.GetByIdAsync(toolId)).ReturnsAsync(tools.SingleOrDefault(x => x.Id.Equals(toolId)));
        _mockToolService.Setup(ts => ts.DeleteAsync(toolId)).ReturnsAsync(!toolsAfterDeletion.Contains(tools[3]));
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.DeleteToolAsync(toolId);

        // Then
        _mockToolService.Verify(ts => ts.GetByIdAsync(toolId), Times.Once);
        _mockToolService.Verify(ts => ts.DeleteAsync(toolId), Times.Once);
        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
    }

    [Fact]
    public async void DeleteToolAsync_ShouldReturnBadRequentWhenToolIsFoundButCannotBeDeleted()
    {
        // Given
        List<ToolResponse>? tools = new()
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

        int toolId = 4;
        _mockToolService!.Setup(ts => ts.GetByIdAsync(toolId)).ReturnsAsync(tools.SingleOrDefault(x => x.Id.Equals(toolId)));
        _mockToolService.Setup(ts => ts.DeleteAsync(toolId)).ReturnsAsync(false);
        _controller = new(_mockToolService.Object);

        // When
        IActionResult result = await _controller.DeleteToolAsync(toolId);

        // Then
        _mockToolService.Verify(ts => ts.GetByIdAsync(toolId), Times.Once);
        _mockToolService.Verify(ts => ts.DeleteAsync(toolId), Times.Once);
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal($"Tool {toolId} was found but failed to delete.", response.Value);
    }
}