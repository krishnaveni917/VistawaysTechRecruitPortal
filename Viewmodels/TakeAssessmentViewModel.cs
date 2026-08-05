namespace VistaWaysTechRecruitPortal.ViewModels
{
    public class TakeAssessmentViewModel
    {
        public int QuestionId { get; set; }

        public string Question { get; set; } = "";

        public string OptionA { get; set; } = "";

        public string OptionB { get; set; } = "";

        public string OptionC { get; set; } = "";

        public string OptionD { get; set; } = "";

        public string SelectedAnswer { get; set; } = "";
    }
}