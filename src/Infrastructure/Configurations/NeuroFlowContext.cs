using Domain.Entities;
using Domain.Entities.Profiles;
using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations;

public class NeuroFlowContext : DbContext
{
    public NeuroFlowContext(DbContextOptions<NeuroFlowContext> options)
    : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<ProfessionalProfile> ProfessionalProfiles => Set<ProfessionalProfile>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();
    public DbSet<ThoughtEntry> ThoughtEntries => Set<ThoughtEntry>();
    public DbSet<ThoughtCategory> ThoughtCategories => Set<ThoughtCategory>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ClinicalAssessment> ClinicalAssessments => Set<ClinicalAssessment>();
    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<TokenAudit> TokenAudits => Set<TokenAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NeuroFlowContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
