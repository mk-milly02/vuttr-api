using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.infrastructure.mapping;
using vuttr_api.services;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.services;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<IAuthenticationManager> _mockAuthenticationManager;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _mockUserManager = new
        (
            new Mock<IUserStore<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object
        );

        _mockAuthenticationManager = new(MockBehavior.Loose);
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _userService = new UserService(_mockUserManager.Object, _mockAuthenticationManager.Object, _mapper);
    }

    #region Test Data

    private readonly List<User> users = new()
    {
        new()
        {
            UserName = "testosterone.testosterone",
            Email = "test@test.com"
        },
        new()
        {
            UserName = "testing.testing",
            Email = "testosterone@testosterone.com",
            PasswordSalt = "abc"
        },
        new()
        {
            UserName = "no-coverage",
            Email = "no@info.com"
        }
    };

    #endregion

    [Fact]
    public async Task AlreadyExistsAsync_ShouldReturnTrueForExistingEmail()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "test.test",
            Email = "test@test.com",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByEmailAsync(newUser.Email)).ReturnsAsync(users.SingleOrDefault(u => u.Email!.Equals(newUser.Email)));
        _mockUserManager.Setup(mum => mum.FindByNameAsync(newUser.Username)).ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(newUser.Username)));

        // When
        bool actual = await _userService.AlreadyExistsAsync(newUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByEmailAsync(newUser.Email), Times.Once);
        _mockUserManager.Verify(mum => mum.FindByNameAsync(newUser.Username), Times.Once);
        Assert.True(actual);
    }

    [Fact]
    public async Task AlreadyExistsAsync_ShouldReturnTrueForExistingUsername()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "testosterone.testosterone",
            Email = "tested@tested.com",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByEmailAsync(newUser.Email)).ReturnsAsync(users.SingleOrDefault(u => u.Email!.Equals(newUser.Email)));
        _mockUserManager.Setup(mum => mum.FindByNameAsync(newUser.Username)).ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(newUser.Username)));

        // When
        bool actual = await _userService.AlreadyExistsAsync(newUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByEmailAsync(newUser.Email), Times.Once);
        _mockUserManager.Verify(mum => mum.FindByNameAsync(newUser.Username), Times.Once);
        Assert.True(actual);
    }

    [Fact]
    public async Task AlreadyExistsAsync_ShouldReturnTrueForExistingUsernameAndEmail()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "testosterone.testosterone",
            Email = "test@test.com",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByEmailAsync(newUser.Email)).ReturnsAsync(users.SingleOrDefault(u => u.Email!.Equals(newUser.Email)));
        _mockUserManager.Setup(mum => mum.FindByNameAsync(newUser.Username)).ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(newUser.Username)));

        // When
        bool actual = await _userService.AlreadyExistsAsync(newUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByEmailAsync(newUser.Email), Times.Once);
        _mockUserManager.Verify(mum => mum.FindByNameAsync(newUser.Username), Times.Once);
        Assert.True(actual);
    }

    [Fact]
    public async Task AlreadyExistsAsync_ShouldReturnFalse()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "testament.testament",
            Email = "testament@testament.com",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByEmailAsync(newUser.Email)).ReturnsAsync(users.SingleOrDefault(u => u.Email!.Equals(newUser.Email)));
        _mockUserManager.Setup(mum => mum.FindByNameAsync(newUser.Username)).ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(newUser.Username)));

        // When
        bool actual = await _userService.AlreadyExistsAsync(newUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByEmailAsync(newUser.Email), Times.Once);
        _mockUserManager.Verify(mum => mum.FindByNameAsync(newUser.Username), Times.Once);
        Assert.False(actual);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ShouldReturnANullToken()
    {
        // Given
        UserForAuthentication authUser = new()
        {
            Username = "test.test",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByNameAsync(authUser.Username))
                        .ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(authUser.Username)));

        // When
        (string? token, DateTime expiration) = await _userService.AuthenticateUserAsync(authUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByNameAsync(authUser.Username), Times.Once);
        Assert.Null(token);
        Assert.Equal(expiration, DateTime.MinValue);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ShouldReturnAToken()
    {
        // Given
        UserForAuthentication authUser = new()
        {
            Username = "testing.testing",
            Password = "test123"
        };

        _mockUserManager.Setup(mum => mum.FindByNameAsync(authUser.Username))
                        .ReturnsAsync(users.SingleOrDefault(u => u.UserName!.Equals(authUser.Username)));
        _mockAuthenticationManager.Setup(mam => mam.Combine(authUser.Password, It.IsAny<string>())).Returns("test123abc");
        _mockAuthenticationManager.Setup(mam => mam.ValidateUserAsync(It.IsAny<string>(), "test123abc"))
                                  .ReturnsAsync(("thistoken", DateTime.MaxValue));

        // When
        (string? token, DateTime expiration) = await _userService.AuthenticateUserAsync(authUser);

        // Then
        _mockUserManager.Verify(mum => mum.FindByNameAsync(authUser.Username), Times.Once);
        _mockAuthenticationManager.Verify(mam => mam.Combine(authUser.Password, It.IsAny<string>()), Times.Once);
        _mockAuthenticationManager.Verify(mam => mam.ValidateUserAsync(It.IsAny<string>(), "test123abc"), Times.Once);

        Assert.NotNull(token);
        Assert.Equal("thistoken", token);
        Assert.Equal(expiration, DateTime.MaxValue);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNull()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "no-coverage",
            Password = "console.readline();",
            Email = "no@info.com"
        };

        IdentityError[]? identityErrors = new IdentityError[]
        {
            new IdentityError()
            {
                Code = "Username already exists",
                Description = "Username already exists"
            }
        };

        _mockAuthenticationManager.Setup(mam => mam.GenerateSalt()).Returns("iodatedsalt");
        _mockAuthenticationManager.Setup(mam => mam.Combine(newUser.Password, "iodatedsalt")).Returns(newUser.Password + "iodatedsalt");
        _mockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<User>(), newUser.Password + "iodatedsalt")).ReturnsAsync(IdentityResult.Failed(identityErrors));

        // When
        UserResponse? actual = await _userService.CreateUserAsync(newUser);

        // Then
        _mockAuthenticationManager.Verify(mam => mam.GenerateSalt(), Times.Once);
        _mockAuthenticationManager.Verify(mam => mam.Combine(newUser.Password, "iodatedsalt"), Times.Once);
        _mockUserManager.Verify(mum => mum.CreateAsync(It.IsAny<User>(), newUser.Password + "iodatedsalt"), Times.Once);

        Assert.Null(actual);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNewlyAddedUser()
    {
        // Given
        UserForRegisteration newUser = new()
        {
            Username = "no-coverage",
            Password = "console.readline();",
            Email = "no@info.com"
        };

        _mockAuthenticationManager.Setup(mam => mam.GenerateSalt()).Returns("iodatedsalt");
        _mockAuthenticationManager.Setup(mam => mam.Combine(newUser.Password, "iodatedsalt")).Returns(newUser.Password + "iodatedsalt");
        _mockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<User>(), newUser.Password + "iodatedsalt")).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(mum => mum.FindByEmailAsync(newUser.Email)).ReturnsAsync(users.SingleOrDefault(u => u.Email!.Equals(newUser.Email)));

        // When
        UserResponse? actual = await _userService.CreateUserAsync(newUser);

        // Then
        _mockAuthenticationManager.Verify(mam => mam.GenerateSalt(), Times.Once);
        _mockAuthenticationManager.Verify(mam => mam.Combine(newUser.Password, "iodatedsalt"), Times.Once);
        _mockUserManager.Verify(mum => mum.CreateAsync(It.IsAny<User>(), newUser.Password + "iodatedsalt"), Times.Once);
        _mockUserManager.Verify(mum => mum.FindByEmailAsync(newUser.Email), Times.Once);

        Assert.NotNull(actual);
    }
}