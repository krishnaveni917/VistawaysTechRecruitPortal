using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string CandidateId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
