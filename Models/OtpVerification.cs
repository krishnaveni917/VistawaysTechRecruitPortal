using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public string Purpose { get; set; } = string.Empty; // Registration, PasswordReset
    }
}
