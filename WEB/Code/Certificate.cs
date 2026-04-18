using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Website3.Web.Models;

namespace Website3.Web.Code
{
    public static class Certificates
    {
        public static string GetEncryptionCertificatePath(AppSettings appSettings) =>
            Path.Combine(appSettings.RootPath, "encryption-certificate.pfx");

        public static string GetSigningCertificatePath(AppSettings appSettings) =>
            Path.Combine(appSettings.RootPath, "signing-certificate.pfx");

        public static void CreateEncryptionCertificate(AppSettings appSettings)
        {
            using var algorithm = RSA.Create(2048);

            var subject = new X500DistinguishedName("CN=Fabrikam Encryption Certificate");
            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

            using var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddYears(2));

            File.WriteAllBytes(
                GetEncryptionCertificatePath(appSettings),
                certificate.Export(X509ContentType.Pfx, appSettings.CertificatePassword));
        }

        public static void CreateSigningCertificate(AppSettings appSettings)
        {
            using var algorithm = RSA.Create(2048);

            var subject = new X500DistinguishedName("CN=Fabrikam Signing Certificate");
            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));

            using var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddYears(2));

            File.WriteAllBytes(
                GetSigningCertificatePath(appSettings),
                certificate.Export(X509ContentType.Pfx, appSettings.CertificatePassword));
        }
    }

    public static class CertificateHelper
    {
        public static X509Certificate2 GetCertificate(AppSettings appSettings)
        {
            var certificatePath = Path.Combine(appSettings.RootPath, "certificate.pfx");

            if (!File.Exists(certificatePath))
            {
                using var certificate = BuildSelfSignedServerCertificate(appSettings.SiteName);

                File.WriteAllBytes(
                    certificatePath,
                    certificate.Export(X509ContentType.Pfx, appSettings.CertificatePassword));
            }

            return X509CertificateLoader.LoadPkcs12FromFile(
                certificatePath,
                appSettings.CertificatePassword,
                X509KeyStorageFlags.DefaultKeySet);
        }

        public static X509Certificate2 BuildSelfSignedServerCertificate(string certificateName)
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName("localhost");

            var distinguishedName = new X500DistinguishedName($"CN={certificateName}");

            using var rsa = RSA.Create(2048);

            var request = new CertificateRequest(
                distinguishedName,
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.DigitalSignature,
                    false));

            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") },
                    false));

            request.CertificateExtensions.Add(sanBuilder.Build());

            var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddDays(3650));

            // Windows-only property
            if (OperatingSystem.IsWindows())
            {
                certificate.FriendlyName = certificateName;
            }

            return certificate;
        }
    }
}