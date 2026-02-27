using KazmirukEDMS.Data;
using KazmirukEDMS.DTOs;
using KazmirukEDMS.Models;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KazmirukEDMS.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _db;

        // Сервис работы с документами (CRUD + версии). Использует DbContext напрямую.
        // В более крупном решении бизнес-логика должна быть разделена на отдельные слои и покрыта тестами.
        public DocumentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DocumentDto> CreateDocumentAsync(DocumentCreateDto dto, string? createdById = null)
        {
            var doc = new Document
            {
                Title = dto.Title,
                RegistrationNumber = dto.RegistrationNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };

            _db.Documents.Add(doc);
            await _db.SaveChangesAsync();

            return Map(doc);
        }

        public async Task<bool> DeleteDocumentAsync(Guid id)
        {
            var doc = await _db.Documents.FindAsync(id);
            if (doc == null) return false;

            _db.Documents.Remove(doc);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentDto?> GetDocumentAsync(Guid id)
        {
            var doc = await _db.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null) return null;
            return Map(doc);
        }

        public async Task<PagedResult<DocumentDto>> GetDocumentsAsync(int page = 1, int pageSize = 50)
        {
            var query = _db.Documents.AsNoTracking().OrderByDescending(d => d.CreatedAt);
            var total = await query.CountAsync();
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<DocumentDto>
            {
                Items = list.Select(Map),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<DocumentDto>> GetDocumentsForUserAsync(string? userName, bool isAdmin, int page = 1, int pageSize = 50)
        {
            var query = _db.Documents.AsNoTracking().OrderByDescending(d => d.CreatedAt);
            if (!isAdmin && !string.IsNullOrEmpty(userName))
            {
                query = query.Where(d => d.CreatedById == userName).OrderByDescending(d => d.CreatedAt);
            }

            var total = await query.CountAsync();
            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<DocumentDto>
            {
                Items = list.Select(Map),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DocumentDto?> UpdateDocumentAsync(Guid id, DocumentUpdateDto dto)
        {
            var doc = await _db.Documents.FindAsync(id);
            if (doc == null) return null;

            if (!string.IsNullOrEmpty(dto.Title)) doc.Title = dto.Title!;
            if (dto.RegistrationNumber != null) doc.RegistrationNumber = dto.RegistrationNumber;
            if (dto.Status.HasValue) doc.Status = dto.Status.Value;

            await _db.SaveChangesAsync();
            return Map(doc);
        }

        public async Task<DocumentVersionDto> AddVersionAsync(Guid documentId, DocumentVersionCreateDto dto, string? createdById = null)
        {
            var doc = await _db.Documents.Include(d => d.Versions).FirstOrDefaultAsync(d => d.Id == documentId);
            if (doc == null) throw new InvalidOperationException("Document not found");

            var nextVersion = (doc.Versions?.Count ?? 0) + 1;

            var version = new DocumentVersion
            {
                DocumentId = documentId,
                VersionNumber = nextVersion,
                FilePath = dto.FilePath,
                MimeType = dto.MimeType,
                Checksum = dto.Checksum,
                Signature = dto.Signature,
                SignedBy = dto.SignedBy,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };

            _db.DocumentVersions.Add(version);
            doc.CurrentVersionId = version.Id;

            await _db.SaveChangesAsync();

            return Map(version);
        }

        public async Task<bool> DeleteVersionAsync(Guid id)
        {
            var v = await _db.DocumentVersions.FindAsync(id);
            if (v == null) return false;

            _db.DocumentVersions.Remove(v);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentVersionDto?> GetVersionAsync(Guid id)
        {
            var v = await _db.DocumentVersions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return null;
            return Map(v);
        }

        public async Task<IEnumerable<DocumentVersionDto>> GetVersionsAsync(Guid documentId)
        {
            var list = await _db.DocumentVersions.AsNoTracking()
                .Where(x => x.DocumentId == documentId)
                .OrderByDescending(x => x.VersionNumber)
                .ToListAsync();
            return list.Select(Map);
        }

        // Mapping helpers
        private static DocumentDto Map(Document d)
        {
            return new DocumentDto
            {
                Id = d.Id,
                Title = d.Title,
                RegistrationNumber = d.RegistrationNumber,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                CreatedById = d.CreatedById,
                CurrentVersionId = d.CurrentVersionId
            };
        }

        private static DocumentVersionDto Map(DocumentVersion v)
        {
            return new DocumentVersionDto
            {
                Id = v.Id,
                DocumentId = v.DocumentId,
                VersionNumber = v.VersionNumber,
                FilePath = v.FilePath,
                MimeType = v.MimeType,
                Checksum = v.Checksum,
                SignedBy = v.SignedBy,
                SignedAt = v.SignedAt,
                CreatedAt = v.CreatedAt,
                CreatedById = v.CreatedById
            };
        }
    }
}
