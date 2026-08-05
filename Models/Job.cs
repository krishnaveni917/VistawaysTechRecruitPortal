using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace VistaWaysTechRecruitPortal.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string EmploymentType { get; set; } = "Full Time";

        public string Experience { get; set; } = string.Empty;

        public string Qualification { get; set; } = string.Empty;

        public decimal? Salary { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime PostedDate { get; set; } = DateTime.Now;
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}