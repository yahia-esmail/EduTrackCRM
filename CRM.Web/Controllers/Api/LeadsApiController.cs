using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Web.Controllers.Api;

[ApiController]
[Route("api/v1/leads")]
public class LeadsApiController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly IConfiguration _configuration;

    public LeadsApiController(ILeadService leadService, IConfiguration configuration)
    {
        _leadService = leadService;
        _configuration = configuration;
    }

    [HttpPost("webhook/meta")]
    [AllowAnonymous]
    public async Task<IActionResult> MetaWebhook([FromBody] MetaLeadWebhookModel model, [FromHeader(Name = "X-Hub-Signature-256")] string? signature)
    {
        var secret = _configuration["MetaWebhookSecret"];
        if (!string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(signature))
        {
            var payload = System.Text.Json.JsonSerializer.Serialize(model);
            if (!VerifyHmacSignature(payload, signature, secret))
            {
                return Unauthorized(new { message = "Invalid webhook signature." });
            }
        }

        var leadDto = new LeadDto
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
            Source = model.Platform.ToLower() == "instagram" ? LeadSource.Instagram : LeadSource.Facebook,
            CampaignName = model.CampaignName,
            AdSetName = model.AdSetName,
            AdName = model.AdName,
            EntryTimestamp = DateTime.UtcNow
        };

        var result = await _leadService.CaptureLeadAsync(leadDto);
        return Ok(new { success = result.Success, message = result.Message });
    }

    [HttpPost("webhook/tiktok")]
    [AllowAnonymous]
    public async Task<IActionResult> TikTokWebhook([FromBody] TikTokLeadWebhookModel model, [FromHeader(Name = "X-TikTok-Signature")] string? signature)
    {
        var secret = _configuration["TikTokWebhookSecret"];
        if (!string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(signature))
        {
            var payload = System.Text.Json.JsonSerializer.Serialize(model);
            if (!VerifyHmacSignature(payload, signature, secret))
            {
                return Unauthorized(new { message = "Invalid webhook signature." });
            }
        }

        var leadDto = new LeadDto
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
            Source = LeadSource.TikTok,
            CampaignName = model.CampaignName,
            AdSetName = model.AdSetName,
            AdName = model.AdName,
            EntryTimestamp = DateTime.UtcNow
        };

        var result = await _leadService.CaptureLeadAsync(leadDto);
        return Ok(new { success = result.Success, message = result.Message });
    }

    [HttpPost("website")]
    [AllowAnonymous]
    public async Task<IActionResult> WebsiteLead([FromBody] LeadDto model)
    {
        model.Source = LeadSource.Website;
        model.EntryTimestamp ??= DateTime.UtcNow;

        var result = await _leadService.CaptureLeadAsync(model);
        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }
        return Ok(new { success = true, message = result.Message });
    }

    private bool VerifyHmacSignature(string payload, string signature, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();

        return computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
    }
}
