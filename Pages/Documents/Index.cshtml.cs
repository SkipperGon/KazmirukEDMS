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
            Paged = await _service.GetDocumentsAsync(page, 20);
        }
    }
}
