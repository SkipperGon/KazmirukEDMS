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

        // Navigation
        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();

        // Current active version
        public Guid? CurrentVersionId { get; set; }
    }
}