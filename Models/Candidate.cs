using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace VistaWaysTechRecruitPortal.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        public string CandidateId { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MobileNumber { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string QualificationType { get; set; } = string.Empty;

        public string Qualification { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public int PassingYear { get; set; }

        public decimal Percentage { get; set; }

        public string UndergraduateUniversity { get; set; } = string.Empty;

        public string UndergraduateLocation { get; set; } = string.Empty;

        public string UndergraduateDegree { get; set; } = string.Empty;

        public string UndergraduateSpecialization { get; set; } = string.Empty;

        public int? UndergraduatePassingYear { get; set; }

        public decimal? UndergraduatePercentage { get; set; }

        public string PostgraduateUniversity { get; set; } = string.Empty;

        public string PostgraduateLocation { get; set; } = string.Empty;

        public string PostgraduateDegree { get; set; } = string.Empty;

        public string PostgraduateSpecialization { get; set; } = string.Empty;

        public int? PostgraduatePassingYear { get; set; }

        public decimal? PostgraduatePercentage { get; set; }

        public string IntermediateCollege { get; set; } = string.Empty;

        public string IntermediateLocation { get; set; } = string.Empty;

        public string IntermediateBoard { get; set; } = string.Empty;
        [Required]
        [StringLength(12)]
        public string IntermediateHallTicket { get; set; } = string.Empty;

        public int? IntermediatePassingYear { get; set; }

        public decimal? IntermediatePercentage { get; set; }

        public string TenthInstitute { get; set; } = string.Empty;

        public string TenthLocation { get; set; } = string.Empty;

        public string TenthBoard { get; set; } = string.Empty;
        [Required]
        [StringLength(12)]
        public string TenthHallTicket { get; set; } = string.Empty;

        public int? TenthPassingYear { get; set; }

        public decimal? TenthPercentage { get; set; }

        public int Experience { get; set; }

        public string ResumePath { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PinCode { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool EmailVerified { get; set; }

        public bool ProfileCompleted { get; set; }
        public bool ProfileReviewed { get; set; }
        public string RecruitmentStatus { get; set; } = RecruitmentStage.Registered;
        public string? ApplicationUserId { get; set; }
        public string Role { get; set; } = "Candidate";
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
