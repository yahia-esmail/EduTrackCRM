using CRM.Domain.Enums;

namespace CRM.Application.DTOs;
{
    public class StudentStatusUpdateDto
    {
        public StudentStatus NewStatus { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
