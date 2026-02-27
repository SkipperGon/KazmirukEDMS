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
        public IEnumerable<DocumentVersionDto> Versions { get; set; } = Enumerable.Empty<DocumentVersionDto>();

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Document = await _service.GetDocumentAsync(id);
            if (Document == null) return NotFound();
            Versions = await _service.GetVersionsAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync(Guid id)
        {
            Document = await _service.GetDocumentAsync(id);
            if (Document == null) return NotFound();
            if (Upload == null || Upload.Length == 0)
            {
                ModelState.AddModelError("Upload", "File not selected");
                Versions = await _service.GetVersionsAsync(id);
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
