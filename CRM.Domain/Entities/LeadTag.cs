using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

public class LeadTag
{
    public Guid LeadId { get; set; }
    public Lead Lead { get; set; }

    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}
