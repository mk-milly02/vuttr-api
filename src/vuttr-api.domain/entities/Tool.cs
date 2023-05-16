namespace vuttr_api.domain.entities;

public class Tool
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }
}