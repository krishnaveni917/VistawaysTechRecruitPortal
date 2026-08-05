using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty; 
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}