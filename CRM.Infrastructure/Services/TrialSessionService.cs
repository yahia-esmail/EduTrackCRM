using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Services
{
    public class TrialSessionService : ITrialSessionService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TrialSessionService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<(bool Success, string Message, TrialSession? Session)> CreateTrialSessionAsync(Guid leadId, TrialSessionCreateDto dto)
        {
            // Validate lead
            var lead = await _context.Leads.FindAsync(leadId);
            if (lead == null)
            {
                return (false, "Lead not found.", null);
            }

            // Validate teacher
            var teacher = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == dto.TeacherId && u.Role == AppRole.Teacher);
            if (teacher == null)
            {
                return (false, "Teacher not found or does not have Teacher role.", null);
            }

            var session = new TrialSession
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                TeacherId = dto.TeacherId,
                ScheduledAt = dto.ScheduledAt,
                Subject = dto.Subject,
                Level = dto.Level,
                Notes = dto.Notes,
                Status = TrialSessionStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TrialSessions.Add(session);
            await _context.SaveChangesAsync();

            // TODO: schedule Hangfire reminder 1 hour before ScheduledAt

            return (true, "Trial session booked successfully.", session);
        }

        public async Task<(bool Success, string Message)> UpdateAttendanceAsync(Guid sessionId, TrialSessionStatus newStatus)
        {
            var session = await _context.TrialSessions.FindAsync(sessionId);
            if (session == null)
                return (false, "Trial session not found.");

            session.Status = newStatus;
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Additional business logic (lead stage updates, follow‑up creation) handled elsewhere.
            return (true, "Attendance updated.");
        }

        public async Task<IReadOnlyList<TrialSession>> GetTrialSessionsAsync()
        {
            return await _context.TrialSessions
                .Include(ts => ts.Lead)
                .Include(ts => ts.Teacher)
                .OrderByDescending(ts => ts.ScheduledAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TrialSession>> GetTrialSessionsFilteredAsync(DateTime? start, DateTime? end, Guid? teacherId, TrialSessionStatus? status, string? subject)
        {
            var query = _context.TrialSessions.AsQueryable();
            if (start.HasValue) query = query.Where(ts => ts.ScheduledAt >= start.Value);
            if (end.HasValue) query = query.Where(ts => ts.ScheduledAt <= end.Value);
            if (teacherId.HasValue) query = query.Where(ts => ts.TeacherId == teacherId.Value);
            if (status.HasValue) query = query.Where(ts => ts.Status == status.Value);
            if (!string.IsNullOrWhiteSpace(subject)) query = query.Where(ts => ts.Subject.Contains(subject));

            return await query
                .Include(ts => ts.Lead)
                .Include(ts => ts.Teacher)
                .OrderByDescending(ts => ts.ScheduledAt)
                .ToListAsync();
        }

        public async Task<TrialSessionReportDto> GetReportAsync(DateTime? start, DateTime? end, Guid? teacherId, string? subject)
        {
            var sessions = await GetTrialSessionsFilteredAsync(start, end, teacherId, null, subject);
            var total = sessions.Count;
            var attended = sessions.Count(s => s.Status == TrialSessionStatus.Attended);
            var missed = sessions.Count(s => s.Status == TrialSessionStatus.Missed);
            var attendanceRate = total > 0 ? (double)attended / total * 100 : 0;

            // Per‑teacher breakdown
            var perTeacher = sessions
                .GroupBy(s => s.TeacherId)
                .Select(g => new TeacherAttendanceDto
                {
                    TeacherId = g.Key,
                    TeacherName = _context.AppUsers.FirstOrDefault(u => u.Id == g.Key)?.FullName ?? "Unknown",
                    Total = g.Count(),
                    Attended = g.Count(s => s.Status == TrialSessionStatus.Attended),
                    Missed = g.Count(s => s.Status == TrialSessionStatus.Missed)
                })
                .ToList();

            return new TrialSessionReportDto
            {
                TotalScheduled = total,
                Attended = attended,
                Missed = missed,
                AttendanceRatePercent = attendanceRate,
                TeacherBreakdown = perTeacher
            };
        }
    }
}
