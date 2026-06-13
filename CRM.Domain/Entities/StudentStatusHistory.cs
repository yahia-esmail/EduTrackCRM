using System;
using CRM.Domain.Enums;
using CRM.Domain.Entities;

namespace CRM.Domain.Entities
{
    public class StudentStatusHistory
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public StudentStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid ActorUserId { get; set; }

        // Navigation properties
        public Student Student { get; set; } = null!;
        public AppUser Actor { get; set; } = null!;
    }
}
