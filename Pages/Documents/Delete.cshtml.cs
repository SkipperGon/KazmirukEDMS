using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KazmirukEDMS.Pages.Documents
{
    public class DeleteModel : PageModel
    {
        private readonly IDocumentService _service;
        public DeleteModel(IDocumentService service)
        {
            _service = service;
        }

        public KazmirukEDMS.DTOs.DocumentDto? Document { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Document = await _service.GetDocumentAsync(id);
            if (Document == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var ok = await _service.DeleteDocumentAsync(id);
            if (!ok) return NotFound();
            return RedirectToPage("Index");
        }
    }
}
