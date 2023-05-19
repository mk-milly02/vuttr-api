using AutoMapper;
using Microsoft.AspNetCore.Identity;
using vuttr_api.domain.dtos;
using vuttr_api.domain.entities;
using vuttr_api.services.contracts;

namespace vuttr_api.services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager, IAuthenticationManager authenticationManager, IMapper mapper)
    {
        _userManager = userManager;
        _authenticationManager = authenticationManager;
        _mapper = mapper;
    }

    public async Task<bool> AlreadyExistsAsync(UserForRegisteration createUser)
    {
        User? existingEmail = await _userManager.FindByEmailAsync(createUser.Email!);
        User? existingUsername = await _userManager.FindByNameAsync(createUser.Username!);

        return existingEmail is not null || existingUsername is not null;
    }

    public async Task<(string? token, DateTime expiration)> AuthenticateUserAsync(UserForAuthentication authenticateUser)
    {
        User? existing = await _userManager.FindByNameAsync(authenticateUser.Username!);

        if (existing is null) return (null, DateTime.UtcNow);

        string saltedPassword = _authenticationManager.Combine(authenticateUser.Password!, existing!.PasswordSalt!);
        return await _authenticationManager.ValidateUserAsync(existing.Email!, saltedPassword);
    }

    public async Task<UserResponse?> CreateUserAsync(UserForRegisteration createUser)
    {
        User toBeAdded = new() { PasswordSalt = _authenticationManager.GenerateSalt() };
        string saltedPassword = _authenticationManager.Combine(createUser.Password!, toBeAdded.PasswordSalt);

        _mapper.Map(createUser, toBeAdded);

        IdentityResult result = await _userManager.CreateAsync(toBeAdded, saltedPassword);

        if (result.Succeeded)
        {
            User? added = await _userManager.FindByEmailAsync(createUser.Email!);
            return _mapper.Map<UserResponse>(added);
        }

        return null;
    }
}