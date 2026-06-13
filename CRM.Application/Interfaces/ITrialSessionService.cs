using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Domain.Entities;
using CRM.Application.DTOs;
using CRM.Domain.Enums;

namespace CRM.Application.Interfaces
{
    public interface ITrialSessionService
    {
        Task<(bool Success, string Message, TrialSession? Session)> CreateTrialSessionAsync(Guid leadId, TrialSessionCreateDto dto);
        Task<(bool Success, string Message)> UpdateAttendanceAsync(Guid sessionId, TrialSessionStatus newStatus);
        Task<IReadOnlyList<TrialSession>> GetTrialSessionsAsync();
        Task<IReadOnlyList<TrialSession>> GetTrialSessionsFilteredAsync(DateTime? start, DateTime? end, Guid? teacherId, TrialSessionStatus? status, string? subject);
        Task<TrialSessionReportDto> GetReportAsync(DateTime? start, DateTime? end, Guid? teacherId, string? subject);
    }
}
