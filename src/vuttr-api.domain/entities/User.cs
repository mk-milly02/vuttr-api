using AspNetCore.Identity.MongoDbCore.Models;

namespace vuttr_api.domain.entities;

public class User : MongoIdentityUser<Guid>
{
    public string? PasswordSalt { get; set; }
}