using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class EmailRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
