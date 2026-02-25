using System.ComponentModel.DataAnnotations;

namespace KazmirukEDMS.Models
{
    public enum DocumentStatus
    {
        Draft = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Archived = 4
    }

    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = null!;

        // Registration number according to ГОСТ
        public string? RegistrationNumber { get; set; }

        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedById { get; set; }

        // Audit
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedById { get; set; }

        // Archiving
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }
        public string? ArchivedById { get; set; }

        // ГОСТ / registry metadata
        public string? DocumentType { get; set; }
        public int? StoragePeriodYears { get; set; }

        // Navigation
        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();

        // Current active version
        public Guid? CurrentVersionId { get; set; }
    }
}