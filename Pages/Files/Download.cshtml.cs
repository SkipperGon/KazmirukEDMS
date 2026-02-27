using KazmirukEDMS.Services.Interfaces;
using KazmirukEDMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KazmirukEDMS.Pages.Files
{
    public class DownloadModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileStorageService _storage;

        public DownloadModel(ApplicationDbContext db, IFileStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var version = await _db.DocumentVersions.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (version == null) return NotFound();

            var full = _storage.GetFullPath(version.FilePath);
            if (!System.IO.File.Exists(full)) return NotFound();

            var mime = version.MimeType ?? "application/octet-stream";
            var fileName = System.IO.Path.GetFileName(full);
            var fs = System.IO.File.OpenRead(full);
            return File(fs, mime, fileName);
        }
    }
}
