using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Infrastructure.Data;

namespace CRM.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TrialController : Controller
    {
        private readonly ITrialSessionService _trialService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public TrialController(ITrialSessionService trialService, UserManager<AppUser> userManager, AppDbContext context)
        {
            _trialService = trialService;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Trial/Book/{leadId}
        [HttpGet("Book/{leadId:guid}")]
        public async Task<IActionResult> Book(Guid leadId)
        {
            var lead = await _context.Leads.FindAsync(leadId);
            if (lead == null)
                return NotFound();

            // Get teachers for dropdown
            var teachers = await _userManager.Users
                .Where(u => u.Role == AppRole.Teacher)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            ViewBag.Teachers = teachers;

            var model = new TrialSessionCreateDto { LeadId = leadId };
            return PartialView("_TrialBookingModal", model);
        }

        // POST: /Trial/Book
        [HttpPost("Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(TrialSessionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, session) = await _trialService.CreateTrialSessionAsync(dto.LeadId, dto);
            if (!success)
                return BadRequest(message);

            // Optionally return JSON for AJAX success
            return Json(new { success = true, message, sessionId = session?.Id });
        }

        // POST: /Trial/Attendance
        [HttpPost("Attendance")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attendance(Guid sessionId, TrialSessionStatus status)
        {
            var (success, message) = await _trialService.UpdateAttendanceAsync(sessionId, status);
            if (!success)
                return BadRequest(message);
            return Json(new { success = true, message });
        }
    }
}
