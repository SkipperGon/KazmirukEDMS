using KazmirukEDMS.DTOs;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KazmirukEDMS.Pages.Documents
{
    public class CreateModel : PageModel
    {
        private readonly IDocumentService _service;

        public CreateModel(IDocumentService service)
        {
            _service = service;
        }

        [BindProperty]
        public DocumentCreateDto Input { get; set; } = new DocumentCreateDto();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var created = await _service.CreateDocumentAsync(Input, User?.Identity?.Name);
            return RedirectToPage("Details", new { id = created.Id });
        }
    }
}
