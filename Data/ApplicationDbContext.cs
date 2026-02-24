using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KazmirukEDMS.Models;
using Microsoft.AspNetCore.Identity;

namespace KazmirukEDMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<DocumentVersion> DocumentVersions { get; set; } = null!;
        public DbSet<WorkflowStep> WorkflowSteps { get; set; } = null!;
        public DbSet<Approval> Approvals { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.HasMany(d => d.Versions).WithOne(v => v.Document).HasForeignKey(v => v.DocumentId);
                entity.Property(d => d.Title).HasMaxLength(512).IsRequired();
                entity.Property(d => d.RegistrationNumber).HasMaxLength(128);
            });

            builder.Entity<DocumentVersion>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.MimeType).HasMaxLength(128);
                entity.Property(v => v.Checksum).HasMaxLength(128);
            });

            builder.Entity<WorkflowStep>(entity =>
            {
                entity.HasKey(w => w.Id);
            });

            builder.Entity<Approval>(entity =>
            {
                entity.HasKey(a => a.Id);
            });
        }
    }
}