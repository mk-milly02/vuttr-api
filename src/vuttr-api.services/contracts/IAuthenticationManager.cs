namespace vuttr_api.services.contracts;

public interface IAuthenticationManager
{
    Task<(string? token, DateTime expiration)> ValidateUserAsync(string email, string saltedPassword);
    string GenerateSalt();
    string Combine(string password, string salt);
}