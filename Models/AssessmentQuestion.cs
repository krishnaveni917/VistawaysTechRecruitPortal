using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class AssessmentQuestion
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; } = "";

        [Required]
        public string OptionA { get; set; } = "";

        [Required]
        public string OptionB { get; set; } = "";

        [Required]
        public string OptionC { get; set; } = "";

        [Required]
        public string OptionD { get; set; } = "";

        [Required]
        public string CorrectAnswer { get; set; } = "";

        public int Marks { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}