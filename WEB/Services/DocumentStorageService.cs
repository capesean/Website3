using Website3.Settings;

namespace Website3.Services
{
    public class DocumentsStorageService : StorageServiceBase
    {
        public DocumentsStorageService(DocumentsSettings settings)
            : base(settings.ConnectionString, ContainerNames.Documents)
        {
        }
    }
}