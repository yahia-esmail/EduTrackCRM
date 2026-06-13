using Microsoft.EntityFrameworkCore;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace CRM.Infrastructure.Services;

public class LeadService : ILeadService
{
    private readonly AppDbContext _context;

    public LeadService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, Lead? Lead)> CaptureLeadAsync(LeadDto dto)
    {
        var normalizedPhone = NormalizePhone(dto.Phone);
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        // Task 5: Deduplication check
        var existingLead = await _context.Leads
            .FirstOrDefaultAsync(l => l.Phone == normalizedPhone || l.Email == normalizedEmail);

        if (existingLead != null)
        {
            // Duplicate found: Log the attempt, skip insertion, update LastSeenAt and source if new
            existingLead.LastSeenAt = DateTime.UtcNow;
            existingLead.UpdatedAt = DateTime.UtcNow;
            
            var log = new DuplicateLeadLog
            {
                Name = dto.Name,
                Phone = normalizedPhone,
                Email = normalizedEmail,
                Source = dto.Source,
                AttemptedAt = DateTime.UtcNow,
                ExistingLeadId = existingLead.Id
            };
            
            _context.DuplicateLeadLogs.Add(log);
            await _context.SaveChangesAsync();

            return (false, "Duplicate lead detected and logged.", existingLead);
        }

        // Create new Lead
        var lead = new Lead
        {
            Name = dto.Name,
            Phone = normalizedPhone,
            Email = normalizedEmail,
            Country = dto.Country,
            City = dto.City,
            Language = dto.Language,
            StudentAge = dto.StudentAge,
            GuardianName = dto.GuardianName,
            Subject = dto.Subject,
            CurrentLevel = dto.CurrentLevel,
            Source = dto.Source,
            CampaignName = dto.CampaignName,
            AdSetName = dto.AdSetName,
            AdName = dto.AdName,
            EntryTimestamp = dto.EntryTimestamp ?? DateTime.UtcNow,
            Stage = LeadStage.NewLead,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        return (true, "Lead captured successfully.", lead);
    }

    private string NormalizePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        // Keep only digits
        var digits = Regex.Replace(phone, @"[^\d]", "");

        // Auto convert to international if starting with 00 or 0
        if (digits.StartsWith("00"))
        {
            digits = digits.Substring(2);
        }
        else if (digits.StartsWith("0") && digits.Length > 1)
        {
            // Assuming local representation, you might prefix standard country code or keep as is.
            // For general normalization we just strip a leading single zero to standardise.
            digits = digits.Substring(1);
        }

        return digits;
    }
}
