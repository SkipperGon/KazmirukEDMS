using KazmirukEDMS.DTOs;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KazmirukEDMS.Pages.Documents
{
    public class EditModel : PageModel
    {
        private readonly IDocumentService _service;

        public EditModel(IDocumentService service)
        {
            _service = service;
        }

        [BindProperty]
        public DocumentUpdateDto Input { get; set; } = new DocumentUpdateDto();

        public Guid Id { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var doc = await _service.GetDocumentAsync(id);
            if (doc == null) return NotFound();
            Id = id;
            Input = new DocumentUpdateDto
            {
                Title = doc.Title,
                RegistrationNumber = doc.RegistrationNumber,
                Status = doc.Status
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid) return Page();
            var updated = await _service.UpdateDocumentAsync(id, Input);
            if (updated == null) return NotFound();
            return RedirectToPage("Details", new { id = updated.Id });
        }
    }
}
