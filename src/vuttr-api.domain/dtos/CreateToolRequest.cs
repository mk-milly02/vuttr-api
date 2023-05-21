using System.ComponentModel.DataAnnotations;

namespace vuttr_api.domain.dtos;

public class CreateToolRequest
{
    [Required(ErrorMessage = "Title is required")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Link is required")]
    [Url(ErrorMessage = "Invalid url")]
    public string? Link { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Tags are required")]
    public List<string>? Tags { get; set; }
}