using System.ComponentModel.DataAnnotations;

namespace KazmirukEDMS.Models
{
    public class DocumentVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DocumentId { get; set; }
        public Document? Document { get; set; }

        public int VersionNumber { get; set; }

        // Physical storage path or URI (could be object storage)
        [Required]
        public string FilePath { get; set; } = null!;

        public string? MimeType { get; set; }

        // SHA-256 checksum
        public string? Checksum { get; set; }

        // Digital signature (e.g., ГОСТ) - store as base64 or binary
        public byte[]? Signature { get; set; }

        public string? SignedBy { get; set; }

        public DateTime? SignedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedById { get; set; }

        public bool IsArchived { get; set; } = false;

        public string? Notes { get; set; }
    }
}