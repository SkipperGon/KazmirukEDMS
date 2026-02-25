namespace KazmirukEDMS.DTOs
{
    public class DocumentVersionCreateDto
    {
        public string FilePath { get; set; } = null!;
        public string? MimeType { get; set; }
        public string? Checksum { get; set; }
        public byte[]? Signature { get; set; }
        public string? SignedBy { get; set; }
    }
}