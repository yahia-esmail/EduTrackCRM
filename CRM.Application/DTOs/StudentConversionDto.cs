namespace CRM.Application.DTOs
{
    public class StudentConversionDto
    {
        public Guid TeacherId { get; set; }
        public int WeeklySessions { get; set; }
        public DateTime StartDate { get; set; }
        // Guardian information (optional)
        public string? GuardianName { get; set; }
        public string? GuardianPhone { get; set; }
        public string? GuardianEmail { get; set; }
    }
}
