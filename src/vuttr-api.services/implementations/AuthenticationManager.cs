using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using vuttr_api.domain.entities;
using vuttr_api.domain.settings;
using vuttr_api.services.contracts;

namespace vuttr_api.services;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly JwtSettings? _settings;
    private User? _user;

    public AuthenticationManager(IConfiguration configuration, UserManager<User> userManager, IMapper mapper)
    {
        _configuration = configuration;
        _userManager = userManager;
        _mapper = mapper;

        _settings = _configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
    }

    public string Combine(string password, string salt)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        byte[] output = new byte[passwordBytes.Length + saltBytes.Length];

        Buffer.BlockCopy(passwordBytes, 0, output, 0, passwordBytes.Length);
        Buffer.BlockCopy(saltBytes, 0, output, passwordBytes.Length, saltBytes.Length);
        return Convert.ToBase64String(output);
    }

    public string GenerateSalt()
    {
        byte[] randomNumberBytes = new byte[32];
        RandomNumberGenerator generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumberBytes);
        return Convert.ToBase64String(randomNumberBytes);
    }

    public async Task<(string? token, DateTime expiration)> ValidateUserAsync(string email, string saltedPassword)
    {
        _user = await _userManager.FindByEmailAsync(email);
        return _user != null && await _userManager.CheckPasswordAsync(_user, saltedPassword)
                ? GenerateToken()
                : (null, DateTime.UtcNow);
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] key = Encoding.UTF8.GetBytes(_settings!.SecretKey!);
        SymmetricSecurityKey secret = new(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private List<Claim> GetClaims()
    {
        List<Claim> claims = new()
        {
            new(JwtRegisteredClaimNames.Sub, _user!.UserName!),
            new(ClaimTypes.Email, _user!.Email!)
        };
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        JwtSecurityToken tokenOptions =
            new
            (_settings!.ValidIssuer, _settings.ValidAudience, claims,
                expires: DateTime.UtcNow.AddMinutes(12),
                signingCredentials: signingCredentials
            );
        return tokenOptions;
    }

    private (string token, DateTime expiration) GenerateToken()
    {
        SigningCredentials signingCredentials = GetSigningCredentials();
        List<Claim> claims = GetClaims();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return (new JwtSecurityTokenHandler().WriteToken(tokenOptions), tokenOptions.ValidTo);
    }
}