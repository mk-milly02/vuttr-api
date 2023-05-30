namespace vuttr_api.domain.settings;

public class JwtSettings
{
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? SecretKey { get; set; }
}