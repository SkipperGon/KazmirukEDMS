namespace KazmirukEDMS.Services.Interfaces
{
    public interface ISignatureService
    {
        /// <summary>
        /// Sign data and return signature bytes
        /// </summary>
        Task<byte[]> SignAsync(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verify signature for data
        /// </summary>
        Task<bool> VerifyAsync(byte[] data, byte[] signature, CancellationToken cancellationToken = default);
    }
}