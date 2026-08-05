using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.ViewModels;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class AssessmentsController : Controller
    {
        private const int ExamDurationSeconds = 30 * 60;

        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AssessmentsController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        private async Task<VistaWaysTechRecruitPortal.Models.Candidate?> GetCurrentCandidateAsync()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return null;

            return await _context.Candidates.FirstOrDefaultAsync(c => c.Email == userEmail);
        }

        public async Task<IActionResult> Index()
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var invitations = await _context.AssessmentInvitations
                .Where(i => i.CandidateId == candidate.Id)
                .OrderByDescending(i => i.SentDate)
                .ToListAsync();

            var results = await _context.AssessmentResults
                .Where(r => r.CandidateId == candidate.Id)
                .OrderByDescending(r => r.SubmittedOn)
                .ToListAsync();

            // Candidates never see scores - only that a submission exists and its status.
            ViewBag.Results = results;

            return View(invitations);
        }

        public async Task<IActionResult> Take(int id)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var invitation = await _context.AssessmentInvitations
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (invitation == null)
            {
                TempData["Error"] = "Assessment invitation not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!invitation.IsActive || invitation.IsCompleted)
            {
                TempData["Error"] = "This assessment is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            if (invitation.ExpiryDate < DateTime.Now)
            {
                TempData["Error"] = "This assessment invitation has expired.";
                return RedirectToAction(nameof(Index));
            }

            bool alreadyTaken = await _context.AssessmentResults
                .AnyAsync(r => r.CandidateId == candidate.Id);

            if (alreadyTaken)
            {
                TempData["Error"] = "You have already completed the assessment.";
                return RedirectToAction(nameof(Index));
            }

            // First time opening the exam - start the authoritative server-side clock.
            if (invitation.StartedOn == null)
            {
                invitation.StartedOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            int remainingSeconds = ExamDurationSeconds - (int)(DateTime.Now - invitation.StartedOn.Value).TotalSeconds;

            if (remainingSeconds <= 0)
            {
                // They came back after time had already run out - finalize with no answers.
                return await FinalizeAsync(candidate, invitation, new List<TakeAssessmentViewModel>(),
                    terminatedForViolation: true, reason: "Time expired before the assessment was submitted.");
            }

            var questions = await _context.AssessmentQuestions
                .Where(q => q.IsActive)
                .Select(q => new TakeAssessmentViewModel
                {
                    QuestionId = q.Id,
                    Question = q.Question,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD
                }).ToListAsync();

            if (!questions.Any())
            {
                TempData["Error"] = "No assessment questions are configured yet. Please contact HR.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.InvitationId = invitation.Id;
            ViewBag.RemainingSeconds = remainingSeconds;

            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(List<TakeAssessmentViewModel> model, int invitationId,
            bool terminatedForViolation = false, string? violationReason = null)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var invitation = await _context.AssessmentInvitations
                .FirstOrDefaultAsync(i => i.Id == invitationId && i.CandidateId == candidate.Id);

            if (invitation == null || !invitation.IsActive || invitation.IsCompleted || invitation.ExpiryDate < DateTime.Now)
            {
                TempData["Error"] = "This assessment link is invalid, expired, or already completed.";
                return RedirectToAction(nameof(Index));
            }

            return await FinalizeAsync(candidate, invitation, model ?? new List<TakeAssessmentViewModel>(),
                terminatedForViolation, violationReason);
        }

        // Kept only so any old links don't 404; candidates are never shown scores,
        // so this simply sends them back to their assessments list.
        public IActionResult Results(int id)
        {
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> FinalizeAsync(VistaWaysTechRecruitPortal.Models.Candidate candidate,
            AssessmentInvitation invitation, List<TakeAssessmentViewModel> model,
            bool terminatedForViolation, string? reason)
        {
            int score = 0;
            int correct = 0;

            foreach (var item in model)
            {
                var question = await _context.AssessmentQuestions.FirstOrDefaultAsync(q => q.Id == item.QuestionId);

                if (question == null)
                    continue;

                bool isCorrect = !string.IsNullOrEmpty(item.SelectedAnswer) && question.CorrectAnswer == item.SelectedAnswer;

                if (isCorrect)
                {
                    score += question.Marks;
                    correct++;
                }

                _context.CandidateAnswers.Add(new CandidateAnswer
                {
                    CandidateId = candidate.Id,
                    QuestionId = item.QuestionId,
                    SelectedAnswer = item.SelectedAnswer ?? string.Empty,
                    IsCorrect = isCorrect,
                    MarksObtained = isCorrect ? question.Marks : 0
                });
            }

            // Score is still recorded for HR/admin reporting - it is simply never
            // surfaced to the candidate (see Index/Results and the Complete page).
            var result = new AssessmentResult
            {
                CandidateId = candidate.Id,
                TotalQuestions = model.Count,
                CorrectAnswers = correct,
                Score = score,
                IsPassed = !terminatedForViolation && score >= 5,
                WasTerminatedForViolation = terminatedForViolation,
                TerminationReason = terminatedForViolation ? reason : null
            };

            _context.AssessmentResults.Add(result);

            invitation.IsCompleted = true;
            invitation.IsActive = false;
            invitation.CompletedOn = DateTime.Now;
            invitation.TerminatedForViolation = terminatedForViolation;
            invitation.ViolationReason = terminatedForViolation ? reason : null;

            candidate.RecruitmentStatus = terminatedForViolation
                ? RecruitmentStage.AssessmentTerminated
                : RecruitmentStage.AssessmentCompleted;

            await _context.SaveChangesAsync();

            // Immediately end the authenticated session (submission, timeout, or violation)
            // and hand off to the shared, anonymous, generic thank-you page.
            await _signInManager.SignOutAsync();

            return RedirectToAction("Complete", "Assessment", new { area = "" });
        }
    }
}
