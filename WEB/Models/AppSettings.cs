﻿namespace WEB.Models
{
    public class AppSettings
    {
        public string RootUrl { get; set; }
        public string RootPath { get; set; }
        public string WebRootPath { get; set; }
        public string SiteName { get; set; }
        public string CertificatePassword { get; set; }
        public bool IsDevelopment { get; set; }
        public bool UseApplicationInsights { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public AzureBlobStorage AzureBlobStorage { get; set; }
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryMinutes { get; set; }

        internal bool UseAzureDataProtection
        {
            get
            {
                return !IsDevelopment
                    && !string.IsNullOrEmpty(AzureBlobStorage?.TenantId)
                    && !string.IsNullOrEmpty(AzureBlobStorage?.ClientId)
                    && !string.IsNullOrEmpty(AzureBlobStorage?.ClientSecret)
                    && !string.IsNullOrEmpty(AzureBlobStorage?.AzureBlobStorageUrl);
            }
        }

        public Settings GetDbSettings(ApplicationDbContext db)
        {
            return db.Settings.Single();
        }
    }

    public class EmailSettings
    {
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string SubstitutionEmailAddress { get; set; }
        public string EmailToErrors { get; set; }
        public bool SendEmails { get; set; }
        public bool SendErrorEmails { get; set; }
        public string SMTP { get; set; }
        public int SMTPPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SSL { get; set; }

    }

    public class AzureBlobStorage
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AzureBlobStorageUrl { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
