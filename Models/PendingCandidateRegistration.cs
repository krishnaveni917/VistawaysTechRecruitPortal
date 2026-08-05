using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class PendingCandidateRegistration
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime ExpiresOn { get; set; }

        public bool IsVerified { get; set; }
    }
}
