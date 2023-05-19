using vuttr_api.domain.dtos;

namespace vuttr_api.services.contracts;

public interface IUserService
{
    Task<UserResponse?> CreateUserAsync(UserForRegisteration createUser);
    Task<bool> AlreadyExistsAsync(UserForRegisteration createUser);
    Task<(string? token, DateTime expiration)> AuthenticateUserAsync(UserForAuthentication authenticateUser);
}