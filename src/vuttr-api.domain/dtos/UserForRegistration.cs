using System.ComponentModel.DataAnnotations;

namespace vuttr_api.domain.dtos;

public class UserForRegistration
{
    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    public string? Password { get; set; }
}