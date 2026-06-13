using CRM.Application.DTOs;
using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface ILeadService
{
    Task<(bool Success, string Message, Lead? Lead)> CaptureLeadAsync(LeadDto dto);
}
