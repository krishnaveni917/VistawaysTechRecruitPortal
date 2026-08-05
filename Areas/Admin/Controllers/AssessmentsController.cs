using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CredentialEmailService _credentialEmailService;

        public AssessmentsController(ApplicationDbContext context, CredentialEmailService credentialEmailService)
        {
            _context = context;
            _credentialEmailService = credentialEmailService;
        }

        public async Task<IActionResult> Index()
        {
            var questions = await _context.AssessmentQuestions
                .Where(q => q.IsActive)
                .ToListAsync();
            return View(questions);
        }

        public IActionResult Create()
        {
            return View(new AssessmentQuestion { Marks = 1 });
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            var form = await Request.ReadFormAsync();

            string GetVal(string key) => form[key].ToString().Trim();

            var question = new AssessmentQuestion
            {
                Question = GetVal("Question"),
                OptionA = GetVal("OptionA"),
                OptionB = GetVal("OptionB"),
                OptionC = GetVal("OptionC"),
                OptionD = GetVal("OptionD"),
                CorrectAnswer = GetVal("CorrectAnswer").ToUpper(),
                Marks = int.TryParse(GetVal("Marks"), out var marks) && marks > 0 ? marks : 1
            };

            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(question.Question)) missing.Add("Question");
            if (string.IsNullOrWhiteSpace(question.OptionA)) missing.Add("Option A");
            if (string.IsNullOrWhiteSpace(question.OptionB)) missing.Add("Option B");
            if (string.IsNullOrWhiteSpace(question.OptionC)) missing.Add("Option C");
            if (string.IsNullOrWhiteSpace(question.OptionD)) missing.Add("Option D");
            if (string.IsNullOrWhiteSpace(question.CorrectAnswer)) missing.Add("Correct Answer");

            if (missing.Any())
            {
                TempData["Error"] = "Please fill in: " + string.Join(", ", missing);
                return View(question);
            }

            if (question.CorrectAnswer != "A" && question.CorrectAnswer != "B" &&
                question.CorrectAnswer != "C" && question.CorrectAnswer != "D")
            {
                TempData["Error"] = "Correct Answer must be A, B, C, or D.";
                return View(question);
            }

            question.IsActive = true;

            try
            {
                _context.AssessmentQuestions.Add(question);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not save the question: " + ex.Message;
                return View(question);
            }

            TempData["Success"] = "Question created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var question = await _context.AssessmentQuestions.FindAsync(id);
            if (question == null)
                return NotFound();

            return View(question);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id)
        {
            var existing = await _context.AssessmentQuestions.FindAsync(id);
            if (existing == null)
                return NotFound();

            var form = await Request.ReadFormAsync();
            string GetVal(string key) => form[key].ToString().Trim();

            var newQuestion = GetVal("Question");
            var newA = GetVal("OptionA");
            var newB = GetVal("OptionB");
            var newC = GetVal("OptionC");
            var newD = GetVal("OptionD");
            var newCorrect = GetVal("CorrectAnswer").ToUpper();
            var marksOk = int.TryParse(GetVal("Marks"), out var newMarks) && newMarks > 0;

            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(newQuestion)) missing.Add("Question");
            if (string.IsNullOrWhiteSpace(newA)) missing.Add("Option A");
            if (string.IsNullOrWhiteSpace(newB)) missing.Add("Option B");
            if (string.IsNullOrWhiteSpace(newC)) missing.Add("Option C");
            if (string.IsNullOrWhiteSpace(newD)) missing.Add("Option D");
            if (newCorrect != "A" && newCorrect != "B" && newCorrect != "C" && newCorrect != "D")
                missing.Add("Correct Answer (must be A, B, C, or D)");

            if (missing.Any())
            {
                TempData["Error"] = "Please fill in: " + string.Join(", ", missing);
                existing.Question = newQuestion;
                existing.OptionA = newA;
                existing.OptionB = newB;
                existing.OptionC = newC;
                existing.OptionD = newD;
                existing.CorrectAnswer = newCorrect;
                return View(existing);
            }

            existing.Question = newQuestion;
            existing.OptionA = newA;
            existing.OptionB = newB;
            existing.OptionC = newC;
            existing.OptionD = newD;
            existing.CorrectAnswer = newCorrect;
            existing.Marks = marksOk ? newMarks : existing.Marks;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Question updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.AssessmentQuestions.FindAsync(id);
            if (question == null)
                return NotFound();

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.AssessmentQuestions.FindAsync(id);
            if (question != null)
            {
                question.IsActive = false;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Question deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Invitations()
        {
            var invitations = await _context.AssessmentInvitations
                .OrderByDescending(i => i.SentDate)
                .ToListAsync();
            return View(invitations);
        }

        public IActionResult CreateInvitation()
        {
            ViewBag.Candidates = _context.Candidates.ToList();
            return View();
        }

        [HttpPost]
        [ActionName("CreateInvitation")]
        public async Task<IActionResult> CreateInvitationPost()
        {
            var form = await Request.ReadFormAsync();
            int.TryParse(form["CandidateId"].ToString(), out var candidateId);

            if (candidateId == 0)
            {
                TempData["Error"] = "Please select a candidate.";
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(new AssessmentInvitation());
            }

            var candidate = await _context.Candidates.FindAsync(candidateId);

            if (candidate == null)
            {
                TempData["Error"] = "Selected candidate could not be found.";
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(new AssessmentInvitation());
            }

            bool alreadyInvited = await _context.AssessmentInvitations
                .AnyAsync(i => i.CandidateId == candidateId && i.IsActive && !i.IsCompleted);

            if (alreadyInvited)
            {
                TempData["Error"] = "This candidate already has an active assessment invitation.";
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(new AssessmentInvitation { CandidateId = candidateId });
            }

            var invitation = new AssessmentInvitation
            {
                CandidateId = candidateId,
                AssessmentToken = Guid.NewGuid().ToString("N"),
                SentDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                IsActive = true,
                IsCompleted = false
            };

            try
            {
                _context.AssessmentInvitations.Add(invitation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not send the invitation: " + ex.Message;
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(invitation);
            }

            // Automatically email the candidate their assessment link. This does not roll back
            // the invitation if delivery fails - the admin is warned so they can resend or
            // share the link manually instead.
            var assessmentUrl = Url.Action("Start", "Assessment", new { token = invitation.AssessmentToken, area = "" }, Request.Scheme)
                ?? string.Empty;

            try
            {
                await _credentialEmailService.SendAssessmentInvitationEmailAsync(
                    candidate.Email, candidate.FirstName, assessmentUrl, invitation.ExpiryDate);

                TempData["Success"] = "Assessment invitation sent and emailed to the candidate successfully.";
            }
            catch (Exception)
            {
                TempData["Success"] = "Assessment invitation created, but the email could not be delivered. " +
                    "Please share the assessment link with the candidate manually.";
            }

            return RedirectToAction(nameof(Invitations));
        }

        public async Task<IActionResult> Results()
        {
            var results = await _context.AssessmentResults
                .OrderByDescending(r => r.SubmittedOn)
                .ToListAsync();
            return View(results);
        }
    }
}
