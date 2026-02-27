using KazmirukEDMS.Services.Interfaces;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace KazmirukEDMS.Services
{
    public class SignatureOptions
    {
        public string Algorithm { get; set; } = "GOST3411WITHECGOST3410"; // default algorithm name (may be adjusted)
        public string? PrivateKeyPath { get; set; }
        public string? PublicKeyPath { get; set; }
    }

    public class BouncyCastleSignatureService : ISignatureService
    {
        private readonly SignatureOptions _opts;
        private readonly AsymmetricKeyParameter? _privateKey;
        private readonly AsymmetricKeyParameter? _publicKey;

        public BouncyCastleSignatureService(IOptions<SignatureOptions> opts)
        {
            _opts = opts.Value;

            if (!string.IsNullOrEmpty(_opts.PrivateKeyPath) && File.Exists(_opts.PrivateKeyPath))
            {
                using var reader = File.OpenText(_opts.PrivateKeyPath);
                var pr = new PemReader(reader);
                var keyObj = pr.ReadObject();
                if (keyObj is AsymmetricCipherKeyPair kp)
                    _privateKey = kp.Private;
                else if (keyObj is AsymmetricKeyParameter akp)
                    _privateKey = akp;
            }

            if (!string.IsNullOrEmpty(_opts.PublicKeyPath) && File.Exists(_opts.PublicKeyPath))
            {
                using var reader = File.OpenText(_opts.PublicKeyPath);
                var pr = new PemReader(reader);
                var keyObj = pr.ReadObject();
                if (keyObj is AsymmetricKeyParameter akp)
                    _publicKey = akp;
                else if (keyObj is Org.BouncyCastle.X509.X509Certificate cert)
                    _publicKey = cert.GetPublicKey();
            }
        }

        // BouncyCastleSignatureService is a proof-of-concept implementation of a signature provider
        // that uses BouncyCastle to sign/verify arbitrary byte arrays. In production this layer
        // should be replaced with an implementation that integrates with certified SKZI (e.g., CryptoPro)
        // and proper key/certificate storage.

        public Task<byte[]> SignAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            if (_privateKey == null) throw new InvalidOperationException("Private key not configured for signing");
            var signer = SignerUtilities.GetSigner(_opts.Algorithm);
            signer.Init(true, _privateKey);
            signer.BlockUpdate(data, 0, data.Length);
            var sig = signer.GenerateSignature();
            return Task.FromResult(sig);
        }

        public Task<bool> VerifyAsync(byte[] data, byte[] signature, CancellationToken cancellationToken = default)
        {
            if (_publicKey == null) throw new InvalidOperationException("Public key not configured for verification");
            var signer = SignerUtilities.GetSigner(_opts.Algorithm);
            signer.Init(false, _publicKey);
            signer.BlockUpdate(data, 0, data.Length);
            var ok = signer.VerifySignature(signature);
            return Task.FromResult(ok);
        }
    }
}
