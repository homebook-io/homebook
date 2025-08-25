using System.ComponentModel.DataAnnotations;

namespace HomeBook.Frontend.Models.Setup;

public class HomebookConfigurationViewModel
{
    [Required(ErrorMessage = "InstanceName is required")]
    [StringLength(255, ErrorMessage = "InstanceName cannot exceed 255 characters")]
    public string InstanceName { get; set; } = string.Empty;
}
