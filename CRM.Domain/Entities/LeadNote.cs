using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities
{
    public class LeadNote
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid LeadId { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        // Plain text notes; attachments allowed via separate entity
        public bool HasAttachment => Attachment != null;

        public Guid? AttachmentId { get; set; }
        public LeadNoteAttachment Attachment { get; set; }

        [Required]
        public string AuthorId { get; set; } = string.Empty; // references AppUser

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LeadId))]
        public Lead Lead { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public AppUser Author { get; set; }
    }

    public class LeadNoteAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public byte[] Data { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
