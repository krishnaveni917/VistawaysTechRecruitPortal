using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;

namespace VistaWaysTechRecruitPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AdminController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalCandidates = _context.Candidates.Count();
            ViewBag.PendingAssessment = _context.AssessmentInvitations.Count(x => x.IsActive && !x.IsCompleted);
            ViewBag.Registered = _context.Candidates.Count();
            ViewBag.Assessments = _context.AssessmentResults.Count();
            ViewBag.Interviews = _context.Interviews.Count();
            ViewBag.Documents = _context.CandidateDocuments.Count(x => x.IsVerified);
            ViewBag.Offers = _context.OfferLetters.Count();
            ViewBag.Accepted = _context.OfferLetters.Count(x => x.OfferStatus == "Accepted");

            return View();
        }

        public IActionResult Candidates()
        {
            var candidates = _context.Candidates.ToList();

            return View(candidates);
        }

        public async Task<IActionResult> ReviewProfile(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
                return NotFound();

            candidate.ProfileReviewed = true;
            candidate.RecruitmentStatus = RecruitmentStage.ProfileUnderReview;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile reviewed. You can now send the secure assessment invitation.";

            return RedirectToAction("Candidates");
        }

        public async Task<IActionResult> SendAssessment(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
                return NotFound();

            string token = Guid.NewGuid().ToString("N");

            var activeInvitations = _context.AssessmentInvitations
                .Where(x => x.CandidateId == candidate.Id && x.IsActive && !x.IsCompleted);

            foreach (var activeInvitation in activeInvitations)
            {
                activeInvitation.IsActive = false;
            }

            AssessmentInvitation invitation = new AssessmentInvitation
            {
                CandidateId = candidate.Id,
                AssessmentToken = token,
                SentDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7)
            };

            _context.AssessmentInvitations.Add(invitation);
            candidate.RecruitmentStatus = RecruitmentStage.AssessmentInvited;

            await _context.SaveChangesAsync();

            var assessmentLink = Url.Action(
                "Start",
                "Assessment",
                new { token },
                Request.Scheme);

            await _emailService.SendEmailAsync(
                candidate.Email,
                "VistaWays Tech - Secure Assessment Invitation",
                $@"
                    <h2>VistaWays Tech Assessment Invitation</h2>
                    <p>Dear {candidate.FirstName},</p>
                    <p>Your profile has been reviewed. Please click the secure link below and authenticate using your Candidate ID and password.</p>
                    <p><strong>Candidate ID:</strong> {candidate.CandidateId}</p>
                    <p><a href='{assessmentLink}' style='background:#0d6efd;color:white;padding:10px 18px;text-decoration:none;border-radius:5px;'>Start Assessment</a></p>
                    <p>This link expires on {invitation.ExpiryDate:dd-MMM-yyyy HH:mm}.</p>
                    <p>Regards,<br/>VistaWays Tech HR</p>");

            TempData["Success"] = "Secure assessment invitation sent successfully.";

            return RedirectToAction("Candidates");
        }

        public IActionResult Documents()
        {
            var documents = _context.CandidateDocuments.ToList();

            return View(documents);
        }

        public IActionResult VerifyDocument(int id)
        {
            var document = _context.CandidateDocuments.Find(id);

            if (document == null)
                return NotFound();

            document.IsVerified = true;

            var candidate = _context.Candidates.Find(document.CandidateId);

            if (candidate != null)
            {
                candidate.RecruitmentStatus = RecruitmentStage.DocumentsVerified;
            }

            _context.SaveChanges();

            TempData["Success"] = "Documents verified successfully.";

            return RedirectToAction(nameof(Documents));
        }
    }
}
