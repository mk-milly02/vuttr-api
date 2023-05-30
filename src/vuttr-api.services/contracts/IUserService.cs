using vuttr_api.domain.dtos;

namespace vuttr_api.services.contracts;

public interface IUserService
{
    Task<UserViewModel?> CreateUserAsync(UserForRegistration createUser);
    Task<bool> AlreadyExistsAsync(UserForRegistration createUser);
    Task<AuthenticationResponse?> AuthenticateUserAsync(UserForAuthentication authenticateUser);
}