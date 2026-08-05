using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class OfferLetter
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public decimal Salary { get; set; }

        public string Designation { get; set; } = "";

        public string Department { get; set; } = "";

        public DateTime JoiningDate { get; set; }

        public DateTime OfferDate { get; set; } = DateTime.Now;

        public string OfferStatus { get; set; } = "Pending";

        public string OfferPdfPath { get; set; } = "";
    }
}