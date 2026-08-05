using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var candidates = _context.Candidates.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                candidates = candidates.Where(c =>
                    c.FirstName.Contains(search) ||
                    c.LastName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.CandidateId.Contains(search));
            }

            return View(await candidates
                .OrderByDescending(c => c.RegistrationDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
                return NotFound();

            return View(candidate);
        }
    }
}