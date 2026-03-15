using KazmirukEDMS.DTOs;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace KazmirukEDMS.Pages.Documents
{
    public class DetailsModel : PageModel
    {
        private readonly IDocumentService _service;
        private readonly IFileStorageService _storage;

        public DetailsModel(IDocumentService service, IFileStorageService storage)
        {
            _service = service;
            _storage = storage;
        }

        public DocumentDto? Document { get; set; }

        // View model for rendering version list with computed metadata (icon, size, download url)
        public class VersionViewModel
        {
            public Guid Id { get; set; }
            public int VersionNumber { get; set; }
            public string FileName { get; set; } = null!;
            public string IconUrl { get; set; } = "/assets/icons/file-types/unknown.svg";
            public string SizeText { get; set; } = "-";
            public string? MimeType { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? CreatedById { get; set; }
            public string DownloadUrl { get; set; } = string.Empty;
            public bool HasSignature { get; set; }
            public string? Checksum { get; set; }
        }

        public IList<VersionViewModel> Versions { get; set; } = new List<VersionViewModel>();

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Document = await _service.GetDocumentAsync(id);
            if (Document == null) return NotFound();

            var vers = await _service.GetVersionsAsync(id);
            Versions = vers.Select(v => BuildVersionViewModel(v)).ToList();
            return Page();
        }

        private VersionViewModel BuildVersionViewModel(DocumentVersionDto v)
        {
            var vm = new VersionViewModel
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                FileName = System.IO.Path.GetFileName(v.FilePath),
                MimeType = v.MimeType,
                CreatedAt = v.CreatedAt,
                CreatedById = v.CreatedById,
                Checksum = v.Checksum,
                DownloadUrl = Url.Page("/Files/Download", new { id = v.Id }) ?? $"/Files/Download?id={v.Id}",
                HasSignature = !string.IsNullOrEmpty(v.SignedBy) || v.SignedAt.HasValue
            };

            try
            {
                var full = _storage.GetFullPath(v.FilePath);
                if (System.IO.File.Exists(full))
                {
                    var fi = new System.IO.FileInfo(full);
                    vm.SizeText = FormatSize(fi.Length);
                }
            }
            catch
            {
                // ignore IO errors, fallback to default size text
            }

            // Map extension to icon file names. Icons in the repo use uppercase names (e.g. PDF.svg, DOCX.svg)
            var ext = System.IO.Path.GetExtension(vm.FileName).TrimStart('.').ToLowerInvariant();
            var iconMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "mp3", "MP3.svg" }, { "mp4", "MP4.svg" }, { "odg", "ODG.svg" }, { "odp", "ODP.svg" }, { "ods", "ODS.svg" },
                { "pdf", "PDF.svg" }, { "png", "PNG.svg" }, { "rtf", "RTF.svg" }, { "shp", "SHP.svg" }, { "json", "JSON.svg" },
                { "7z", "7Z.svg" }, { "avi", "AVI.svg" }, { "csv", "CSV.svg" }, { "doc", "DOC.svg" }, { "docx", "DOCX.svg" },
                { "dwg", "DWG.svg" }, { "dxf", "DXF.svg" }, { "html", "HTML.svg" }, { "htm", "HTML.svg" }, { "jpg", "JPG.svg" }, { "jpeg", "JPG.svg" }
            };

            if (iconMap.TryGetValue(ext, out var iconFile))
            {
                var iconPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", iconFile);
                if (System.IO.File.Exists(iconPath)) vm.IconUrl = $"/assets/icons/file-types/{iconFile}";
            }
            else
            {
                // special-case PDF/A -> use PDF-A.svg when checksum or mimetype indicates PDF/A is not available here;
                // we still fallback to PDF.svg if PDF-A not present
                if (ext == "pdf")
                {
                    var pdfa = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", "PDF-A.svg");
                    if (System.IO.File.Exists(pdfa)) vm.IconUrl = "/assets/icons/file-types/PDF-A.svg";
                    else
                    {
                        var pdf = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", "PDF.svg");
                        if (System.IO.File.Exists(pdf)) vm.IconUrl = "/assets/icons/file-types/PDF.svg";
                    }
                }
                else
                {
                    // fallback: try uppercase extension filename
                    var up = ext.ToUpperInvariant() + ".svg";
                    var upath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "icons", "file-types", up);
                    if (System.IO.File.Exists(upath)) vm.IconUrl = $"/assets/icons/file-types/{up}";
                }
            }

            return vm;
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:F1} KB";
            double mb = kb / 1024.0;
            if (mb < 1024) return $"{mb:F1} MB";
            double gb = mb / 1024.0;
            return $"{gb:F2} GB";
        }

        public async Task<IActionResult> OnPostUploadAsync(Guid id)
        {
            Document = await _service.GetDocumentAsync(id);
            if (Document == null) return NotFound();
            if (Upload == null || Upload.Length == 0)
            {
                ModelState.AddModelError("Upload", "Файл не выбран");
                var vers = await _service.GetVersionsAsync(id);
                Versions = vers.Select(v => BuildVersionViewModel(v)).ToList();
                return Page();
            }

            var saved = await _storage.SaveFileAsync(id, Upload);

            var dto = new DocumentVersionCreateDto
            {
                FilePath = saved.FileName, // store relative path
                MimeType = Upload.ContentType,
                Checksum = saved.Checksum,
                SignedBy = null
            };

            await _service.AddVersionAsync(id, dto, User?.Identity?.Name);

            return RedirectToPage();
        }
    }
}
