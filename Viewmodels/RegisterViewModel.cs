using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string VerificationToken { get; set; } = "";

        [Required]
        public string MobileNumber { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";

        
        [Display(Name = "Highest Qualification")]
        public string QualificationType { get; set; } = ""; // UG, PG, Both

        public string Qualification { get; set; } = "";

        public string Specialization { get; set; } = "";

        public int PassingYear { get; set; }

        public decimal Percentage { get; set; }

       
        [Display(Name = "Undergraduate university")]
        public string UndergraduateUniversity { get; set; } = "";

        public string UndergraduateLocation { get; set; } = "";

      
        [Display(Name = "Undergraduate degree")]
        public string UndergraduateDegree { get; set; } = "";

       
        [Display(Name = "Undergraduate specialization")]
        public string UndergraduateSpecialization { get; set; } = "";

        
        [Display(Name = "UG passing year")]
        public int? UndergraduatePassingYear { get; set; }


       
        [Display(Name = "UG percentage")]
        public decimal? UndergraduatePercentage { get; set; }

        [Display(Name = "Postgraduate university")]
        public string PostgraduateUniversity { get; set; } = "";

        public string PostgraduateLocation { get; set; } = "";

        [Display(Name = "Postgraduate degree")]
        public string PostgraduateDegree { get; set; } = "";

        [Display(Name = "Postgraduate specialization")]
        public string PostgraduateSpecialization { get; set; } = "";

        [Display(Name = "PG passing year")]
        public int? PostgraduatePassingYear { get; set; }

        
        [Display(Name = "PG percentage")]
        public decimal? PostgraduatePercentage { get; set; }

        [Required]
        [Display(Name = "Intermediate college")]
        public string IntermediateCollege { get; set; } = "";

        public string IntermediateLocation { get; set; } = "";

        [Required]
        [Display(Name = "Intermediate board")]
        public string IntermediateBoard { get; set; } = "";

        [Required]
        [StringLength(12)]
        [Display(Name = "Intermediate hall ticket")]
        public string IntermediateHallTicket { get; set; } = "";

        [Required]
        [Display(Name = "Intermediate passing year")]
        public int? IntermediatePassingYear { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Intermediate percentage")]
        public decimal? IntermediatePercentage { get; set; }

        [Required]
        [Display(Name = "10th School")]
        public string TenthInstitute { get; set; } = "";

        public string TenthLocation { get; set; } = "";

        [Required]
        [Display(Name = "10th board")]
        public string TenthBoard { get; set; } = "";

        [Required]
        [StringLength(12)]
        [Display(Name = "10th hall ticket")]
        public string TenthHallTicket { get; set; } = "";

        [Required]
        [Display(Name = "10th passing year")]
        public int? TenthPassingYear { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "10th percentage")]
        public decimal? TenthPercentage { get; set; }

        public int Experience { get; set; }

        public string Address { get; set; } = "";

        public string City { get; set; } = "";

        public string State { get; set; } = "";

        public string Country { get; set; } = "";

        public string PinCode { get; set; } = "";

        public IFormFile? Resume { get; set; }
    }
}
