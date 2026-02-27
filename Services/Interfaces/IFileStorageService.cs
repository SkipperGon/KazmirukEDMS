namespace KazmirukEDMS.Services.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Save uploaded file to local storage in a folder named by documentId. Filename will be a GUID with original extension.
        /// Returns tuple (fullPath, fileName, checksumHex)
        /// </summary>
        Task<(string FullPath, string FileName, string Checksum)> SaveFileAsync(Guid documentId, IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get full file path by stored relative path (as saved in DB)
        /// </summary>
        string GetFullPath(string storedPath);

        /// <summary>
        /// Delete file if exists
        /// </summary>
        Task<bool> DeleteFileAsync(string storedPath);
    }
}