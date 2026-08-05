using System.ComponentModel.DataAnnotations;

namespace VistaWaysTechRecruitPortal.Models
{
    public class CandidateDocument
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public string AadhaarPath { get; set; } = "";

        public string PanPath { get; set; } = "";

        public string DegreeCertificatePath { get; set; } = "";

        public string ExperienceCertificatePath { get; set; } = "";

        public string PassportPhotoPath { get; set; } = "";

        public bool IsVerified { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.Now;
    }
}