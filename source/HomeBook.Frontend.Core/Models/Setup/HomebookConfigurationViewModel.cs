using System.ComponentModel.DataAnnotations;

namespace HomeBook.Frontend.Core.Models.Setup;

public class HomebookConfigurationViewModel
{
    [Required(ErrorMessage = "InstanceName is required")]
    [StringLength(255, ErrorMessage = "InstanceName cannot exceed 255 characters")]
    public string InstanceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "InstanceDefaultLang is required")]
    [StringLength(3, ErrorMessage = "InstanceDefaultLang cannot exceed 3 characters")]
    public string InstanceDefaultLang { get; set; } = string.Empty;
}
