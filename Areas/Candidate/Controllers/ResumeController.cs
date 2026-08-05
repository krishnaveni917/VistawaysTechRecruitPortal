using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class ResumeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResumeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(candidate);
        }

        public async Task<IActionResult> Upload()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile resume)
        {
            if (resume == null || resume.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            string[] allowedExtensions = { ".pdf", ".doc", ".docx" };
            string extension = Path.GetExtension(resume.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Only PDF, DOC, and DOCX files are allowed.";
                return RedirectToAction(nameof(Index));
            }

            if (resume.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "File size must be less than 5 MB.";
                return RedirectToAction(nameof(Index));
            }

            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/resumes");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(resume.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }

            candidate.ResumePath = "/uploads/resumes/" + uniqueFileName;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Resume uploaded successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
