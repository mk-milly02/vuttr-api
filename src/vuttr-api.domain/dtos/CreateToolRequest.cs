namespace vuttr_api.domain.dtos;

public class CreateToolRequest
{
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }
}