namespace CRM.Application.DTOs
{
    public class TrialSessionReportDto
    {
        public int TotalSessions { get; set; }
        public int Attended { get; set; }
        public int Missed { get; set; }
        public int Cancelled { get; set; }
        public int Rescheduled { get; set; }
        // Add more fields as needed for reporting
    }
}
