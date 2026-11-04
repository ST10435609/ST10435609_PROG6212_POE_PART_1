// start of BlobStorageService
using Azure.Storage.Blobs;

namespace RaceDay.Api.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blob_service_client;

        public BlobStorageService(IConfiguration configuration)
        {
            var connection_string = configuration["AzureBlobStorage:ConnectionString"];
            _blob_service_client = new BlobServiceClient(connection_string);
        }

        public async Task<string> UploadAsync(string container_name, IFormFile file)
        {
            var container_client = _blob_service_client.GetBlobContainerClient(container_name);

            // GUID-based name so two uploads never collide or silently overwrite each other
            var blob_name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blob_client = container_client.GetBlobClient(blob_name);

            using var stream = file.OpenReadStream();
            await blob_client.UploadAsync(stream, overwrite: true);

            return blob_client.Uri.ToString();
        }
    }
}
// end of BlobStorageService