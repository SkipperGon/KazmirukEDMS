using KazmirukEDMS.DTOs;
using KazmirukEDMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KazmirukEDMS.Pages.Documents
{
    public class IndexModel : PageModel
    {
        private readonly IDocumentService _service;

        public IndexModel(IDocumentService service)
        {
            _service = service;
        }

        public PagedResult<DocumentDto> Paged { get; set; } = new PagedResult<DocumentDto>();

        public async Task OnGetAsync(int page = 1)
        {
            var userName = User?.Identity?.Name;
            var isAdmin = User?.IsInRole("Administrator") ?? false;
            Paged = await _service.GetDocumentsForUserAsync(userName, isAdmin, page, 20);
        }
    }
}
