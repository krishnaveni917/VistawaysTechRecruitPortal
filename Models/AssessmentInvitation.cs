using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class AssessmentInvitation
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public string AssessmentToken { get; set; } = string.Empty;

        public DateTime SentDate { get; set; } = DateTime.Now;

        public DateTime ExpiryDate { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CompletedOn { get; set; }

        // Set when the candidate accepts the instructions/consent page.
        // Used as the authoritative start time for the exam timer so a page
        // refresh cannot reset the countdown.
        public DateTime? ConsentGivenOn { get; set; }

        public DateTime? StartedOn { get; set; }

        // Set when the exam is auto-submitted because of a proctoring violation
        // (tab switch, window blur, or exiting fullscreen).
        public bool TerminatedForViolation { get; set; }

        public string? ViolationReason { get; set; }
    }
}
