using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DocumentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(
            IFormFile aadhaar,
            IFormFile pan,
            IFormFile degree,
            IFormFile experience,
            IFormFile photo)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (candidate == null)
                return NotFound();

            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Documents");

            Directory.CreateDirectory(uploadFolder);

            async Task<string> SaveFile(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    return "";

                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(uploadFolder, fileName);

                using var stream = new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);

                return "/Documents/" + fileName;
            }

            var document = new CandidateDocument
            {
                CandidateId = candidate.Id,
                AadhaarPath = await SaveFile(aadhaar),
                PanPath = await SaveFile(pan),
                DegreeCertificatePath = await SaveFile(degree),
                ExperienceCertificatePath = await SaveFile(experience),
                PassportPhotoPath = await SaveFile(photo),
                IsVerified = false
            };

            _context.CandidateDocuments.Add(document);
            candidate.RecruitmentStatus = RecruitmentStage.DocumentsUploaded;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Documents uploaded successfully.";

            return RedirectToAction("Dashboard", "Candidate");
        }
    }
}
