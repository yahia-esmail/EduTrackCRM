using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Application.DTOs;
using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;
{
    public interface IStudentService
    {
        /// <summary>
        /// Convert a lead (must be in Subscribed stage) to a student.
        /// </summary>
        Task<Student> ConvertLeadAsync(Guid leadId, StudentConversionDto conversionDto);

        /// <summary>
        /// Retrieve a student by its Id.
        /// </summary>
        Task<StudentDto?> GetStudentAsync(Guid studentId);

        /// <summary>
        /// Update the status of a student, recording history.
        /// </summary>
        Task UpdateStatusAsync(Guid studentId, StudentStatusUpdateDto statusDto, Guid actorUserId);

        /// <summary>
        /// Get all inactive students (On Hold, Inactive, Dropped).
        /// </summary>
        Task<IReadOnlyCollection<StudentDto>> GetInactiveStudentsAsync();

        /// <summary>
        /// Export inactive students to Excel (EPPlus) and return a byte array.
        /// </summary>
        Task<byte[]> ExportInactiveAsync();
    }
}
