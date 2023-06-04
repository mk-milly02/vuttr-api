using Microsoft.AspNetCore.Mvc;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.presentation.controllers;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private AuthenticationController? _controller;

    public AuthenticationControllerTests()
    {
        _mockUserService = new(MockBehavior.Loose);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequestForInvalidModelState()
    {
        // Given
        UserForRegistration user = new()
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
        UserForRegistration user = new()
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
        UserForRegistration user = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        UserViewModel? userResponse = null;

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
        UserForRegistration user = new()
        {
            Username = "imagine.dragons",
            Email = "imagine@music.com",
            Password = "imagine1234"
        };

        UserViewModel? userResponse = new()
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
        UserViewModel? addedUser = Assert.IsAssignableFrom<UserViewModel?>(response.Value);
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
        IActionResult response = await _controller.Authenticate(user);

        // Then
        Assert.NotNull(response);
        BadRequestObjectResult responseObject = Assert.IsAssignableFrom<BadRequestObjectResult>(response);
        SerializableError error = Assert.IsType<SerializableError>(responseObject.Value);
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

        AuthenticationResponse? authenticationResponse = null;

        _mockUserService.Setup(mus => mus.AuthenticateUserAsync(user)).ReturnsAsync(authenticationResponse);
        _controller = new(_mockUserService.Object);

        // When
        IActionResult response = await _controller.Authenticate(user);

        // Then
        _mockUserService.Verify(mus => mus.AuthenticateUserAsync(user), Times.Once);

        Assert.IsAssignableFrom<UnauthorizedResult>(response);
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

        AuthenticationResponse? authenticationResponse = new() { Token = "abc123", Expiration = DateTime.MaxValue };

        _mockUserService.Setup(mus => mus.AuthenticateUserAsync(user)).ReturnsAsync(authenticationResponse);
        _controller = new(_mockUserService.Object);

        // When
        IActionResult response = await _controller.Authenticate(user);

        // Then
        _mockUserService.Verify(mus => mus.AuthenticateUserAsync(user), Times.Once);

        OkObjectResult responseObject = Assert.IsAssignableFrom<OkObjectResult>(response);
        Assert.IsAssignableFrom<AuthenticationResponse?>(responseObject.Value);
    }
}