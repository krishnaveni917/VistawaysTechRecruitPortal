using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        public int CandidateId { get; set; }

        public Candidate? Candidate { get; set; } 

        [Required]
        public int JobId { get; set; }

        public Job? Job { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Applied";
    }
}