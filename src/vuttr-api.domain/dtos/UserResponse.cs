namespace vuttr_api.domain.dtos;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedOn { get; set; }
}