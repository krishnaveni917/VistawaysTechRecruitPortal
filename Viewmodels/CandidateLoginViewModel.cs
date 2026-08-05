using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class CandidateLoginViewModel
    {
        [Required(ErrorMessage = "Candidate ID is required")]
        public string CandidateId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
