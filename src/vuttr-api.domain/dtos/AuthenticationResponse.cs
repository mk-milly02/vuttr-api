namespace vuttr_api.domain.dtos;

public class AuthenticationResponse
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}