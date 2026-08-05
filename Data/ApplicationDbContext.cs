using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using VistaWaysTechRecruitPortal.Models;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VistawaysTechRecruitPortal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }

        public DbSet<PendingCandidateRegistration> PendingCandidateRegistrations { get; set; }

        public DbSet<AssessmentInvitation> AssessmentInvitations { get; set; }

        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }

        public DbSet<CandidateAnswer> CandidateAnswers { get; set; }

        public DbSet<Interview> Interviews { get; set; }
       

        public DbSet<CandidateDocument> CandidateDocuments { get; set; }

        public DbSet<OfferLetter> OfferLetters { get; set; }

        public DbSet<University> Universities { get; set; }

        public DbSet<OtpVerification> OtpVerifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId);
        }
    }


}
