using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<DuplicateLeadLog> DuplicateLeadLogs => Set<DuplicateLeadLog>();
    public DbSet<LeadActivity> LeadActivities => Set<LeadActivity>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<LeadNote> LeadNotes => Set<LeadNote>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<LeadTag> LeadTags => Set<LeadTag>();
    public DbSet<TrialSession> TrialSessions => Set<TrialSession>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentStatusHistory> StudentStatusHistories => Set<StudentStatusHistory>();
    public DbSet<StudentContactAttempt> StudentContactAttempts => Set<StudentContactAttempt>();
    public DbSet<LeadNoteAttachment> LeadNoteAttachments => Set<LeadNoteAttachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure indexes
        builder.Entity<Lead>(entity =>
        {
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.AssignedUserId);
            entity.HasIndex(e => e.Stage);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Country);

            entity.HasOne(e => e.AssignedUser)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<DuplicateLeadLog>(entity =>
        {
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.AttemptedAt);

            entity.HasOne(e => e.ExistingLead)
                  .WithMany()
                  .HasForeignKey(e => e.ExistingLeadId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<LeadActivity>(entity =>
        {
            entity.HasIndex(e => e.LeadId);
            entity.HasIndex(e => e.Timestamp);

            entity.HasOne(e => e.Lead)
                  .WithMany()
                  .HasForeignKey(e => e.LeadId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Student entity
        builder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Lead)
                  .WithMany(l => l.Students)
                  .HasForeignKey(e => e.LeadId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Teacher)
                  .WithMany()
                  .HasForeignKey(e => e.TeacherId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.Status);
        });

        // Configure StudentStatusHistory entity
        builder.Entity<StudentStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.StatusHistories)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Actor)
                  .WithMany()
                  .HasForeignKey(e => e.ActorUserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasIndex(e => e.Timestamp);
        });

        // Configure StudentContactAttempt entity
        builder.Entity<StudentContactAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.ContactAttempts)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.AttemptedAt);
        });


        // Configure FollowUp entity
        builder.Entity<FollowUp>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Lead)
                .WithMany(l => l.FollowUps)
                .HasForeignKey(e => e.LeadId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ReminderDate);
        });
    }
}
