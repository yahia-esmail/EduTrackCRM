namespace CRM.Domain.Enums;

public enum LeadStage
{
    NewLead, Assigned, FirstContact, Contacted, NoAnswer, WrongNumber,
    Busy, RequestedCallback, Interested, RequestedDetails, WhatsAppSent,
    TrialOffered, TrialScheduled, TrialReminderSent, TrialAttended, TrialMissed,
    FollowUpRequired, ParentDiscussing, InterestedButDelayed, NotInterested,
    LostLead, Subscribed, ActiveStudent
}

public enum StudentStatus
{
    NewStudent,
    ActiveStudent,
    TrialStudent,
    OnHold,
    Vacation,
    RenewalFollowUp,
    AtRisk,
    InactiveStudent,
    DroppedStudent,
    TransferredTeacher,
    GraduatedCompletedCourse
}

public enum LeadSource
{
    Facebook, Instagram, Website, WhatsApp, Messenger, TikTok, Manual
}

public enum AppRole
{
    Admin, Manager, TeamLeader, CustomerService, Teacher
}
