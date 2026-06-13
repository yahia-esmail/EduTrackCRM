using System;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities
{
    public class StudentContactAttempt
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public DateTime AttemptedAt { get; set; }
        public string? Medium { get; set; } // e.g., Email, Phone, WhatsApp
        public string? Outcome { get; set; } // optional note about result

        // Navigation property
        public Student Student { get; set; } = null!;
    }
}
