using AutoMapper;
using Microsoft.AspNetCore.Identity;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.services.contracts;

namespace vuttr_api.services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthService _authenticationService;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager,
                       IAuthService authenticationService,
                       IMapper mapper)
    {
        _userManager = userManager;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<bool> AlreadyExistsAsync(UserForRegistration createUser)
    {
        ApplicationUser? existingEmail = await _userManager.FindByEmailAsync(createUser.Email!);
        ApplicationUser? existingUsername = await _userManager.FindByNameAsync(createUser.Username!);

        return existingEmail is not null || existingUsername is not null;
    }

    public async Task<AuthenticationResponse?> AuthenticateUserAsync(UserForAuthentication authenticateUser)
    {
        ApplicationUser? existing = await _userManager.FindByNameAsync(authenticateUser.Username!);

        if (existing is null) return null;

        string saltedPassword = _authenticationService.Combine(authenticateUser.Password!, existing!.PasswordSalt!);
        (string? token, DateTime expires) = await _authenticationService.ValidateUserAsync(existing.Email!, saltedPassword);

        return token is null ? null : new() { Token = token, Expiration = expires };
    }

    public async Task<UserViewModel?> CreateUserAsync(UserForRegistration createUser)
    {
        ApplicationUser toBeAdded = new() { PasswordSalt = _authenticationService.GenerateSalt() };
        string saltedPassword = _authenticationService.Combine(createUser.Password!, toBeAdded.PasswordSalt);

        _mapper.Map(createUser, toBeAdded);

        IdentityResult result = await _userManager.CreateAsync(toBeAdded, saltedPassword);

        if (result.Succeeded)
        {
            ApplicationUser? added = await _userManager.FindByEmailAsync(createUser.Email!);
            return _mapper.Map<UserViewModel>(added);
        }

        return null;
    }
}