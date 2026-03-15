using KazmirukEDMS.DTOs;
using KazmirukEDMS.Models;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KazmirukEDMS.Pages.Documents
{
    public class IndexModel : PageModel
    {
        private readonly IDocumentService _service;
        private readonly IFileStorageService _storage;

        public IndexModel(IDocumentService service, IFileStorageService storage)
        {
            _service = service;
            _storage = storage;
        }

        public PagedResult<DocumentDto> Paged { get; set; } = new PagedResult<DocumentDto>();

        // View model for cards
        public class DocumentCard
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = null!;
            public string? RegistrationNumber { get; set; }
            public DocumentStatus Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? CreatedById { get; set; }
            public string IconUrl { get; set; } = "/assets/icons/file-types/unknown.svg";
        }

        public List<DocumentCard> Cards { get; set; } = new List<DocumentCard>();

        public async Task OnGetAsync(int page = 1)
        {
            var userName = User?.Identity?.Name;
            var isAdmin = User?.IsInRole("Administrator") ?? false;
            Paged = await _service.GetDocumentsForUserAsync(userName, isAdmin, page, 20);

            // Build cards and detect icon by current version extension
            var currentIds = Paged.Items.Where(d => d.CurrentVersionId.HasValue && d.CurrentVersionId.Value != Guid.Empty)
                .Select(d => d.CurrentVersionId!.Value).ToList();

            var versionsMap = await _service.GetVersionsByIdsAsync(currentIds);

            var cards = new List<DocumentCard>();
            foreach (var d in Paged.Items)
            {
                var card = new DocumentCard
                {
                    Id = d.Id,
                    Title = d.Title,
                    RegistrationNumber = d.RegistrationNumber,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    CreatedById = d.CreatedById
                };

                if (d.CurrentVersionId.HasValue && versionsMap.TryGetValue(d.CurrentVersionId.Value, out var v))
                {
                    var ext = System.IO.Path.GetExtension(v.FilePath).TrimStart('.').ToLowerInvariant();
                    card.IconUrl = GetIconUrlForExtension(ext);
                }

                cards.Add(card);
            }

            Cards = cards;
        }

        private string GetIconUrlForExtension(string ext)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "mp3", "MP3.svg" }, { "mp4", "MP4.svg" }, { "odg", "ODG.svg" }, { "odp", "ODP.svg" }, { "ods", "ODS.svg" },
                { "pdf", "PDF.svg" }, { "png", "PNG.svg" }, { "rtf", "RTF.svg" }, { "shp", "SHP.svg" }, { "json", "JSON.svg" },
                { "7z", "7Z.svg" }, { "avi", "AVI.svg" }, { "csv", "CSV.svg" }, { "doc", "DOC.svg" }, { "docx", "DOCX.svg" },
                { "dwg", "DWG.svg" }, { "dxf", "DXF.svg" }, { "html", "HTML.svg" }, { "htm", "HTML.svg" }, { "jpg", "JPG.svg" }, { "jpeg", "JPG.svg" }
            };

            if (map.TryGetValue(ext, out var fn))
            {
                var p = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", fn);
                if (System.IO.File.Exists(p)) return $"/assets/icons/file-types/{fn}";
            }

            // pdf-a special case
            if (ext == "pdf")
            {
                var pdfa = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", "PDF-A.svg");
                if (System.IO.File.Exists(pdfa)) return "/assets/icons/file-types/PDF-A.svg";
                var pdf = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", "PDF.svg");
                if (System.IO.File.Exists(pdf)) return "/assets/icons/file-types/PDF.svg";
            }

            var up = ext.ToUpperInvariant() + ".svg";
            var upath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", up);
            if (System.IO.File.Exists(upath)) return $"/assets/icons/file-types/{up}";

            return "/assets/icons/file-types/unknown.svg";
        }
    }
}
