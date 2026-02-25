using KazmirukEDMS.Models;

namespace KazmirukEDMS.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? RegistrationNumber { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; }
        public Guid? CurrentVersionId { get; set; }
    }
}