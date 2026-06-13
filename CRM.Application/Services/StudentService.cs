using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace CRM.Application.Services;
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student> ConvertLeadAsync(Guid leadId, StudentConversionDto conversionDto)
        {
            var lead = await _context.Leads
                .Include(l => l.Students)
                .FirstOrDefaultAsync(l => l.Id == leadId);
            if (lead == null) throw new ArgumentException("Lead not found.");
            if (lead.Stage != LeadStage.Subscribed) throw new InvalidOperationException("Lead must be in Subscribed stage.");

            var student = new Student
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                TeacherId = conversionDto.TeacherId,
                WeeklySessions = conversionDto.WeeklySessions,
                StartDate = conversionDto.StartDate,
                GuardianName = conversionDto.GuardianName,
                GuardianPhone = conversionDto.GuardianPhone,
                GuardianEmail = conversionDto.GuardianEmail,
                Status = StudentStatus.Active
            };
            _context.Students.Add(student);

            // Update lead stage
            lead.Stage = LeadStage.ActiveStudent;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<StudentDto?> GetStudentAsync(Guid studentId)
        {
            var student = await _context.Students
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return null;
            return new StudentDto
            {
                Id = student.Id,
                LeadId = student.LeadId,
                TeacherId = student.TeacherId,
                WeeklySessions = student.WeeklySessions,
                StartDate = student.StartDate,
                Status = student.Status,
                GuardianName = student.GuardianName,
                GuardianPhone = student.GuardianPhone,
                GuardianEmail = student.GuardianEmail
            };
        }

        public async Task UpdateStatusAsync(Guid studentId, StudentStatusUpdateDto statusDto, Guid actorUserId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) throw new ArgumentException("Student not found.");

            // Record history
            var history = new StudentStatusHistory
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                NewStatus = statusDto.NewStatus,
                Reason = statusDto.Reason,
                Notes = statusDto.Notes,
                Timestamp = DateTime.UtcNow,
                ActorUserId = actorUserId
            };
            _context.StudentStatusHistories.Add(history);

            // Update current status
            student.Status = statusDto.NewStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<StudentDto>> GetInactiveStudentsAsync()
        {
            var inactiveStatuses = new[] { StudentStatus.OnHold, StudentStatus.Inactive, StudentStatus.Dropped };
            var students = await _context.Students
                .Where(s => inactiveStatuses.Contains(s.Status))
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    LeadId = s.LeadId,
                    TeacherId = s.TeacherId,
                    WeeklySessions = s.WeeklySessions,
                    StartDate = s.StartDate,
                    Status = s.Status,
                    GuardianName = s.GuardianName,
                    GuardianPhone = s.GuardianPhone,
                    GuardianEmail = s.GuardianEmail
                })
                .ToListAsync();
            return students;
        }

        public async Task<byte[]> ExportInactiveAsync()
        {
            var students = await GetInactiveStudentsAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("InactiveStudents");
            // Header
            ws.Cells[1, 1].Value = "Id";
            ws.Cells[1, 2].Value = "LeadId";
            ws.Cells[1, 3].Value = "TeacherId";
            ws.Cells[1, 4].Value = "WeeklySessions";
            ws.Cells[1, 5].Value = "StartDate";
            ws.Cells[1, 6].Value = "Status";
            ws.Cells[1, 7].Value = "GuardianName";
            ws.Cells[1, 8].Value = "GuardianPhone";
            ws.Cells[1, 9].Value = "GuardianEmail";

            int row = 2;
            foreach (var s in students)
            {
                ws.Cells[row, 1].Value = s.Id;
                ws.Cells[row, 2].Value = s.LeadId;
                ws.Cells[row, 3].Value = s.TeacherId;
                ws.Cells[row, 4].Value = s.WeeklySessions;
                ws.Cells[row, 5].Value = s.StartDate;
                ws.Cells[row, 6].Value = s.Status.ToString();
                ws.Cells[row, 7].Value = s.GuardianName;
                ws.Cells[row, 8].Value = s.GuardianPhone;
                ws.Cells[row, 9].Value = s.GuardianEmail;
                row++;
            }
            return package.GetAsByteArray();
        }
    }
}
