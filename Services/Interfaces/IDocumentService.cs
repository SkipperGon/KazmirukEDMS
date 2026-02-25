using KazmirukEDMS.Models;
using KazmirukEDMS.DTOs;

namespace KazmirukEDMS.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentDto> CreateDocumentAsync(DocumentCreateDto dto, string? createdById = null);
        Task<DocumentDto?> GetDocumentAsync(Guid id);
        Task<PagedResult<DocumentDto>> GetDocumentsAsync(int page = 1, int pageSize = 50);
        Task<DocumentDto?> UpdateDocumentAsync(Guid id, DocumentUpdateDto dto);
        Task<bool> DeleteDocumentAsync(Guid id);

        Task<DocumentVersionDto> AddVersionAsync(Guid documentId, DocumentVersionCreateDto dto, string? createdById = null);
        Task<DocumentVersionDto?> GetVersionAsync(Guid id);
        Task<IEnumerable<DocumentVersionDto>> GetVersionsAsync(Guid documentId);
        Task<bool> DeleteVersionAsync(Guid id);
    }
}