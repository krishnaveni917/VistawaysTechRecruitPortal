using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class Interview
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public Candidate? Candidate { get; set; }

        [Required]
        public string InterviewType { get; set; } = "";

        [Required]
        public DateTime InterviewDate { get; set; }

        [Required]
        public string InterviewTime { get; set; } = "";

        [Required]
        public string InterviewMode { get; set; } = "";

        public string MeetingLink { get; set; } = "";

        public string Interviewer { get; set; } = "";

        public string Status { get; set; } = "Scheduled";

        public string Feedback { get; set; } = "";

        public int Rating { get; set; }

        public bool IsSelected { get; set; }

        public DateTime? CompletedOn { get; set; }
    }
}