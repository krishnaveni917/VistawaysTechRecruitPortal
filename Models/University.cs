using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class University
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = "India";

        public string Type { get; set; } = string.Empty; // University, College, School, Institute

        public string Website { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
