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
        private readonly IWebHostEnvironment _env;

        public DetailsModel(IDocumentService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
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

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", id.ToString());
            Directory.CreateDirectory(uploadsRoot);
            var fileName = Path.GetFileName(Upload.FileName);
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await Upload.CopyToAsync(stream);
            }

            // compute checksum
            string checksum;
            using (var sha = SHA256.Create())
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                var hash = await sha.ComputeHashAsync(stream);
                checksum = BitConverter.ToString(hash).Replace("-", "");
            }

            var dto = new DocumentVersionCreateDto
            {
                FilePath = filePath,
                MimeType = Upload.ContentType,
                Checksum = checksum,
                SignedBy = null
            };

            await _service.AddVersionAsync(id, dto, User?.Identity?.Name);

            return RedirectToPage();
        }
    }
}
