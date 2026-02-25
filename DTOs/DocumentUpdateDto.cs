using KazmirukEDMS.Models;

namespace KazmirukEDMS.DTOs
{
    public class DocumentUpdateDto
    {
        public string? Title { get; set; }
        public string? RegistrationNumber { get; set; }
        public DocumentStatus? Status { get; set; }
    }
}