using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class OtpVerificationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits")]
        public string Otp { get; set; } = string.Empty;
    }
}
