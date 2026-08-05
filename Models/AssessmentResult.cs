using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class AssessmentResult
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public Candidate? Candidate { get; set; }

        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public int Score { get; set; }

        public DateTime SubmittedOn { get; set; } = DateTime.Now;

        public bool IsPassed { get; set; }

        public bool WasTerminatedForViolation { get; set; }

        public string? TerminationReason { get; set; }
    }
}