using KazmirukEDMS.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace KazmirukEDMS.Services
{
    public class LocalStorageOptions
    {
        public string RootPath { get; set; } = "C:\\edms_storage";
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _root;

        public FileStorageService(IOptions<LocalStorageOptions> options)
        {
            _root = options.Value.RootPath ?? "C:\\edms_storage";
            if (!Path.IsPathRooted(_root))
            {
                // make absolute relative to content root
                _root = Path.GetFullPath(_root);
            }

            Directory.CreateDirectory(_root);
        }

        // NOTE: This implementation stores files on local disk under RootPath.
        // Filenames are GUIDs with original extension to avoid collisions and to not leak original names.
        // The service returns the stored relative path (documentId/file) which should be saved in DB.

        public string GetFullPath(string storedPath)
        {
            if (Path.IsPathRooted(storedPath)) return storedPath;
            return Path.Combine(_root, storedPath);
        }

        public async Task<(string FullPath, string FileName, string Checksum)> SaveFileAsync(Guid documentId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var docDir = Path.Combine(_root, documentId.ToString());
            Directory.CreateDirectory(docDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + ext;
            var fullPath = Path.Combine(docDir, fileName);

            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // compute sha256
            string checksum;
            using (var sha = SHA256.Create())
            using (var fs = System.IO.File.OpenRead(fullPath))
            {
                var hash = await sha.ComputeHashAsync(fs, cancellationToken);
                checksum = BitConverter.ToString(hash).Replace("-", "");
            }

            // storedPath relative to root
            var storedRelative = Path.Combine(documentId.ToString(), fileName);
            return (GetFullPath(storedRelative), storedRelative, checksum);
        }

        public Task<bool> DeleteFileAsync(string storedPath)
        {
            var full = GetFullPath(storedPath);
            if (!System.IO.File.Exists(full)) return Task.FromResult(false);
            System.IO.File.Delete(full);
            return Task.FromResult(true);
        }
    }
}
