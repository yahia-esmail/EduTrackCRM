using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Infrastructure.Data;
using CRM.Web.Models;

namespace CRM.Web.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ILeadService _leadService;
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public LeadsController(ILeadService leadService, AppDbContext context, UserManager<AppUser> userManager)
    {
        _leadService = leadService;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        LeadStage? stage,
        string? country,
        string? agentId,
        LeadSource? source,
        string? search)
    {
        var query = _context.Leads
            .Include(l => l.AssignedUser)
            .AsQueryable();

        // Security level data filtering: CustomerService sees only their assigned records
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Role == AppRole.CustomerService)
        {
            query = query.Where(l => l.AssignedUserId == currentUser.Id);
        }

        // Apply filters
        if (stage.HasValue)
        {
            query = query.Where(l => l.Stage == stage.Value);
        }
        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(l => l.Country == country);
        }
        if (!string.IsNullOrEmpty(agentId))
        {
            query = query.Where(l => l.AssignedUserId == agentId);
        }
        if (source.HasValue)
        {
            query = query.Where(l => l.Source == source.Value);
        }
        if (!string.IsNullOrEmpty(search))
        {
            var cleanSearch = search.Trim().ToLower();
            query = query.Where(l => l.Name.ToLower().Contains(cleanSearch) || 
                                     l.Phone.Contains(cleanSearch) || 
                                     l.Email.ToLower().Contains(cleanSearch));
        }

        var leads = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();

        // Populate filter dropdown data
        ViewBag.Stages = Enum.GetValues<LeadStage>();
        ViewBag.Sources = Enum.GetValues<LeadSource>();
        ViewBag.Countries = await _context.Leads.Select(l => l.Country).Distinct().ToListAsync();
        
        // Agents for filters (Users with Manager, TeamLeader, CS roles)
        ViewBag.Agents = await _userManager.Users
            .Where(u => u.Role == AppRole.CustomerService || u.Role == AppRole.TeamLeader || u.Role == AppRole.Manager)
            .ToListAsync();

        return View(leads);
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int id)
    {
        var lead = await _context.Leads
            .Include(l => l.AssignedUser)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lead == null)
        {
            return NotFound();
        }

        // Security check
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Role == AppRole.CustomerService && lead.AssignedUserId != currentUser.Id)
        {
            return Forbid();
        }

        // Fetch logs
        var timeline = await _context.LeadActivities
            .Where(la => la.LeadId == id)
            .OrderByDescending(la => la.Timestamp)
            .ToListAsync();

        var assignmentLogs = await _context.LeadAssignmentLogs
            .Where(lal => lal.LeadId == id)
            .Include(lal => lal.FromUser)
            .Include(lal => lal.ToUser)
            .OrderByDescending(lal => lal.Timestamp)
            .ToListAsync();

        // CS list for reassignment modal
        ViewBag.Agents = await _userManager.Users
            .Where(u => u.Role == AppRole.CustomerService && u.IsActive)
            .ToListAsync();

        ViewBag.Timeline = timeline;
        ViewBag.AssignmentLogs = assignmentLogs;

        return View(lead);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStage(int id, LeadStage stage)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead == null) return NotFound();

        var prevStage = lead.Stage;
        if (prevStage == stage) return RedirectToAction("Profile", new { id });

        lead.Stage = stage;
        lead.UpdatedAt = DateTime.UtcNow;

        // Log Activity Timeline
        var actor = User.Identity?.Name ?? "System";
        var activity = new LeadActivity
        {
            LeadId = lead.Id,
            ActorName = actor,
            Description = $"Stage changed from {prevStage} to {stage}",
            PreviousStage = prevStage,
            NewStage = stage
        };

        _context.LeadActivities.Add(activity);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Stage updated successfully.";
        return RedirectToAction("Profile", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignAgent(int id, string? agentId)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead == null) return NotFound();

        var prevAgentId = lead.AssignedUserId;
        if (prevAgentId == agentId) return RedirectToAction("Profile", new { id });

        lead.AssignedUserId = agentId;
        lead.UpdatedAt = DateTime.UtcNow;

        var actor = User.Identity?.Name ?? "System";
        var log = new LeadAssignmentLog
        {
            LeadId = lead.Id,
            FromUserId = prevAgentId,
            ToUserId = agentId,
            AssignedByUsername = actor,
            Timestamp = DateTime.UtcNow
        };

        var toUserName = "Unassigned";
        if (!string.IsNullOrEmpty(agentId))
        {
            var toUser = await _userManager.FindByIdAsync(agentId);
            if (toUser != null) toUserName = toUser.FullName;
        }

        var activity = new LeadActivity
        {
            LeadId = lead.Id,
            ActorName = actor,
            Description = $"Lead assigned to {toUserName}"
        };

        _context.LeadAssignmentLogs.Add(log);
        _context.LeadActivities.Add(activity);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Lead assignment updated successfully.";
        return RedirectToAction("Profile", new { id });
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View(new CreateLeadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CreateLeadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new LeadDto
        {
            Name = model.Name,
            Phone = model.Phone,
            Email = model.Email,
            Country = model.Country,
            City = model.City,
            Language = model.Language,
            StudentAge = model.StudentAge,
            GuardianName = model.GuardianName,
            Subject = model.Subject,
            CurrentLevel = model.CurrentLevel,
            Source = model.Source,
            CampaignName = model.CampaignName,
            AdSetName = model.AdSetName,
            AdName = model.AdName,
            EntryTimestamp = DateTime.UtcNow
        };

        var result = await _leadService.CaptureLeadAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // Post-creation Auto Assignment logic (round-robin among active CS agents)
        if (result.Lead != null)
        {
            await AutoAssignLeadAsync(result.Lead);
        }

        TempData["SuccessMessage"] = "Lead added successfully.";
        return RedirectToAction("Add");
    }

    private async Task AutoAssignLeadAsync(Lead lead)
    {
        var csAgents = await _userManager.Users
            .Where(u => u.Role == AppRole.CustomerService && u.IsActive)
            .ToListAsync();

        if (!csAgents.Any()) return;

        // Round-robin: find CS agent with the least assigned leads
        var agentStats = await _context.Leads
            .Where(l => l.AssignedUserId != null)
            .GroupBy(l => l.AssignedUserId)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToListAsync();

        var targetAgent = csAgents
            .Select(agent => new { Agent = agent, Count = agentStats.FirstOrDefault(s => s.AgentId == agent.Id)?.Count ?? 0 })
            .OrderBy(x => x.Count)
            .First().Agent;

        lead.AssignedUserId = targetAgent.Id;
        
        var log = new LeadAssignmentLog
        {
            LeadId = lead.Id,
            FromUserId = null,
            ToUserId = targetAgent.Id,
            AssignedByUsername = "System (Auto-Assign)",
            Timestamp = DateTime.UtcNow
        };

        var activity = new LeadActivity
        {
            LeadId = lead.Id,
            ActorName = "System",
            Description = $"Lead automatically assigned to {targetAgent.FullName} via Round-Robin."
        };

        _context.LeadAssignmentLogs.Add(log);
        _context.LeadActivities.Add(activity);
        await _context.SaveChangesAsync();
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Please upload a valid Excel file.");
            return View();
        }

        var successCount = 0;
        var duplicateCount = 0;
        var errorCount = 0;
        var errors = new List<string>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    ModelState.AddModelError(string.Empty, "No worksheet found in the Excel file.");
                    return View();
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount < 2)
                {
                    ModelState.AddModelError(string.Empty, "Excel file is empty or missing headers.");
                    return View();
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var name = worksheet.Cells[row, 1].Value?.ToString();
                        var phone = worksheet.Cells[row, 2].Value?.ToString();
                        var email = worksheet.Cells[row, 3].Value?.ToString();
                        var country = worksheet.Cells[row, 4].Value?.ToString() ?? "Unknown";
                        var city = worksheet.Cells[row, 5].Value?.ToString() ?? "Unknown";
                        var language = worksheet.Cells[row, 6].Value?.ToString() ?? "Unknown";
                        var studentAgeStr = worksheet.Cells[row, 7].Value?.ToString();
                        var guardianName = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty;
                        var subject = worksheet.Cells[row, 9].Value?.ToString() ?? "General";
                        var currentLevel = worksheet.Cells[row, 10].Value?.ToString() ?? "Beginner";
                        var sourceStr = worksheet.Cells[row, 11].Value?.ToString() ?? "Manual";

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email))
                        {
                            errorCount++;
                            errors.Add($"Row {row}: Name, Phone, and Email are required fields.");
                            continue;
                        }

                        int? studentAge = null;
                        if (int.TryParse(studentAgeStr, out int age))
                        {
                            studentAge = age;
                        }

                        if (!Enum.TryParse(sourceStr, true, out LeadSource source))
                        {
                            source = LeadSource.Manual;
                        }

                        var dto = new LeadDto
                        {
                            Name = name,
                            Phone = phone,
                            Email = email,
                            Country = country,
                            City = city,
                            Language = language,
                            StudentAge = studentAge,
                            GuardianName = guardianName,
                            Subject = subject,
                            CurrentLevel = currentLevel,
                            Source = source,
                            EntryTimestamp = DateTime.UtcNow
                        };

                        var result = await _leadService.CaptureLeadAsync(dto);
                        if (result.Success)
                        {
                            successCount++;
                            if (result.Lead != null)
                            {
                                await AutoAssignLeadAsync(result.Lead);
                            }
                        }
                        else
                        {
                            duplicateCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Row {row}: Unhandled error - {ex.Message}");
                    }
                }
            }
        }

        ViewBag.SuccessCount = successCount;
        ViewBag.DuplicateCount = duplicateCount;
        ViewBag.ErrorCount = errorCount;
        ViewBag.ErrorsList = errors;

        return View();
    }

    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Leads Template");
            
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Phone";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Country";
            worksheet.Cells[1, 5].Value = "City";
            worksheet.Cells[1, 6].Value = "Language";
            worksheet.Cells[1, 7].Value = "StudentAge";
            worksheet.Cells[1, 8].Value = "GuardianName";
            worksheet.Cells[1, 9].Value = "Subject";
            worksheet.Cells[1, 10].Value = "CurrentLevel";
            worksheet.Cells[1, 11].Value = "Source";

            worksheet.Cells[2, 1].Value = "John Doe";
            worksheet.Cells[2, 2].Value = "+1234567890";
            worksheet.Cells[2, 3].Value = "john.doe@example.com";
            worksheet.Cells[2, 4].Value = "United States";
            worksheet.Cells[2, 5].Value = "New York";
            worksheet.Cells[2, 6].Value = "English";
            worksheet.Cells[2, 7].Value = 12;
            worksheet.Cells[2, 8].Value = "Jane Doe";
            worksheet.Cells[2, 9].Value = "Maths";
            worksheet.Cells[2, 10].Value = "Grade 6";
            worksheet.Cells[2, 11].Value = "Manual";

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Leads_Import_Template.xlsx");
        }
    }
}
