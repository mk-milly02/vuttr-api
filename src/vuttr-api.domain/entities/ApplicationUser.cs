using Microsoft.AspNetCore.Identity;

namespace vuttr_api.domain.entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? PasswordSalt { get; set; }
}