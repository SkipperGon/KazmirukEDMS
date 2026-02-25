namespace KazmirukEDMS.DTOs
{
    public class DocumentVersionDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public int VersionNumber { get; set; }
        public string FilePath { get; set; } = null!;
        public string? MimeType { get; set; }
        public string? Checksum { get; set; }
        public string? SignedBy { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; }
    }
}