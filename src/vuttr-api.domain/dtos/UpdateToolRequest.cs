using System.ComponentModel.DataAnnotations;

namespace vuttr_api.domain.dtos;

public class UpdateToolRequest
{
    [Required(ErrorMessage = "Title field is required")]
    [MaxLength(50, ErrorMessage = "Title field should have a maximum of 50 character")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Link field is required")]
    [Url(ErrorMessage = "Invalid url")]
    [MaxLength(ErrorMessage = "Shorten the url to less than 50 characters")]
    public string? Link { get; set; }

    [Required(ErrorMessage = "Description field is required")]
    [MaxLength(2000, ErrorMessage = "Description is too long")]
    public string? Description { get; set; }

    [MinLength(1, ErrorMessage = "A tool should have at least on tag")]
    public List<string>? Tags { get; set; }
}