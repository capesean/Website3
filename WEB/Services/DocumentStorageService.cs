using Website3.Web.Settings;

namespace Website3.Web.Services
{
    public class DocumentsStorageService : StorageServiceBase
    {
        public DocumentsStorageService(DocumentsSettings settings)
            : base(settings.ConnectionString, settings.ContainerName)
        {
        }
    }
}