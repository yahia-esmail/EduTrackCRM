using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid LeadId { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string AuthorId { get; set; } = string.Empty; // references AppUser Id

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LeadId))]
        public Lead Lead { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public AppUser Author { get; set; }
    }
}
