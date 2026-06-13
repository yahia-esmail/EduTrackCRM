using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Domain.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation for many-to-many relationship
        public ICollection<LeadTag> LeadTags { get; set; } = new List<LeadTag>();
    }
}
