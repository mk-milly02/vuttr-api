using Microsoft.AspNetCore.Mvc;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.presentation.controllers;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private UsersController? _controller;

    public UsersControllerTests()
    {
        _mockUserService = new(MockBehavior.Loose);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequestForInvalidModelState()
    {
        // Given
        UserForRegisteration user = new()
        {
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        _controller = new(_mockUserService.Object);
        _controller.ModelState.AddModelError("Username", "Username is required");

        // When
        IActionResult result = await _controller.Resigter(user);

        // Then
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        SerializableError error = Assert.IsType<SerializableError>(response.Value);
        KeyValuePair<string, object> expectedError = new("Username", new[] { "Username is required" });
        Assert.Equal(error.First().Key, expectedError.Key);
        Assert.Equal(error.First().Value, expectedError.Value);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequestForAlreadyExistingUser()
    {
        // Given
        UserForRegisteration user = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        _mockUserService.Setup(mus => mus.AlreadyExistsAsync(user)).ReturnsAsync(true);
        _controller = new(_mockUserService.Object);

        // When
        IActionResult result = await _controller.Resigter(user);

        // Then
        _mockUserService.Verify(mus => mus.AlreadyExistsAsync(user), Times.Once);
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal("User already exists.", response.Value);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequestForFailureToCreateUser()
    {
        // Given
        UserForRegisteration user = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        UserResponse? userResponse = null;

        _mockUserService.Setup(mus => mus.AlreadyExistsAsync(user)).ReturnsAsync(false);
        _mockUserService.Setup(mus => mus.CreateUserAsync(user)).ReturnsAsync(userResponse);
        _controller = new(_mockUserService.Object);

        // When
        IActionResult result = await _controller.Resigter(user);

        // Then
        _mockUserService.Verify(mus => mus.AlreadyExistsAsync(user), Times.Once);
        _mockUserService.Verify(mus => mus.CreateUserAsync(user), Times.Once);

        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        Assert.Equal("Repository failed to create user.", response.Value);
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        // Given
        UserForRegisteration user = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        UserResponse? userResponse = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com"
        };

        _mockUserService.Setup(mus => mus.AlreadyExistsAsync(user)).ReturnsAsync(false);
        _mockUserService.Setup(mus => mus.CreateUserAsync(user)).ReturnsAsync(userResponse);
        _controller = new(_mockUserService.Object);

        // When
        IActionResult result = await _controller.Resigter(user);

        // Then
        _mockUserService.Verify(mus => mus.AlreadyExistsAsync(user), Times.Once);
        _mockUserService.Verify(mus => mus.CreateUserAsync(user), Times.Once);

        Assert.NotNull(result);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        UserResponse? addedUser = Assert.IsAssignableFrom<UserResponse?>(response.Value);
        Assert.NotNull(addedUser);
        Assert.Equal(addedUser.Email, user.Email);
        Assert.Equal(addedUser.Username, user.Username);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnBadRequestForInvalidModelState()
    {
        // Given
        UserForAuthentication user = new()
        {
            Password = "imagine1234"
        };

        _controller = new(_mockUserService.Object);
        _controller.ModelState.AddModelError("Username", "Username is required");

        // When
        IActionResult result = await _controller.Authenticate(user);

        // Then
        Assert.NotNull(result);
        BadRequestObjectResult response = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        SerializableError error = Assert.IsType<SerializableError>(response.Value);
        KeyValuePair<string, object> expectedError = new("Username", new[] { "Username is required" });
        Assert.Equal(error.First().Key, expectedError.Key);
        Assert.Equal(error.First().Value, expectedError.Value);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnUnauthorized()
    {
        // Given
        UserForAuthentication user = new()
        {
            Username = "imagine.dragons",
            Password = "imagine1234"
        };

        _mockUserService.Setup(mus => mus.AuthenticateUserAsync(user)).ReturnsAsync((null, DateTime.MinValue));
        _controller = new(_mockUserService.Object);

        // When
        IActionResult result = await _controller.Authenticate(user);

        // Then
        _mockUserService.Verify(mus => mus.AuthenticateUserAsync(user), Times.Once);
        Assert.NotNull(result);
        Assert.IsAssignableFrom<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnOk()
    {
        // Given
        UserForAuthentication user = new()
        {
            Username = "imagine.dragons",
            Password = "imagine1234"
        };

        _mockUserService.Setup(mus => mus.AuthenticateUserAsync(user)).ReturnsAsync(("null", DateTime.MaxValue));
        _controller = new(_mockUserService.Object);

        // When
        IActionResult result = await _controller.Authenticate(user);

        // Then
        _mockUserService.Verify(mus => mus.AuthenticateUserAsync(user), Times.Once);
        Assert.NotNull(result);
        OkObjectResult response = Assert.IsAssignableFrom<OkObjectResult>(result);
        object x = new { token = "null", expires = DateTime.MaxValue };
        Assert.Equal(x.GetType().GetProperty("token")!.GetValue(x), response.Value!.GetType().GetProperty("token")!.GetValue(response.Value));
        Assert.Equal(x.GetType().GetProperty("expires")!.GetValue(x), response.Value.GetType().GetProperty("expires")!.GetValue(response.Value));
    }
}