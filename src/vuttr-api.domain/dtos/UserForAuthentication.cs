using System.ComponentModel.DataAnnotations;

namespace vuttr_api.domain.dtos;

public class UserForAuthentication
{
    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}