namespace VistaWaysTechRecruitPortal.Models
{
    public class CandidateAnswer
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public int QuestionId { get; set; }

        public string SelectedAnswer { get; set; } = "";

        public bool IsCorrect { get; set; }

        public int MarksObtained { get; set; }
    }
}