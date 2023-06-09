using System.ComponentModel.DataAnnotations;

namespace vuttr_api.domain.dtos;

public class UserForRegistration
{
    [Required(ErrorMessage = "Username is required.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Must have a least 6 characters.")]
    public string? Password { get; set; }
}