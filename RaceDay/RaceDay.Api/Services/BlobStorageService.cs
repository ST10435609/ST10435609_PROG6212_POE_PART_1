// start of BlobStorageService
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services
{
    public class BlobStorageService
    {
        private readonly string? _connection_string;
        private BlobServiceClient? _blob_service_client;

        public BlobStorageService(IConfiguration configuration)
        {
            // just read the value here — don't connect yet, so this service
            // can be safely injected even when no connection string is set
            _connection_string = configuration["AzureBlobStorage:ConnectionString"];
        }

        private BlobServiceClient GetClient()
        {
            if (_blob_service_client == null)
            {
                if (string.IsNullOrWhiteSpace(_connection_string))
                {
                    throw new InvalidOperationException(
                        "Azure Blob Storage connection string is not configured.");
                }

                _blob_service_client = new BlobServiceClient(_connection_string);
            }

            return _blob_service_client;
        }

        public async Task<string> UploadAsync(string container_name, IFormFile file)
        {
            var container_client = GetClient().GetBlobContainerClient(container_name);

            var blob_name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blob_client = container_client.GetBlobClient(blob_name);

            using var stream = file.OpenReadStream();

            var upload_options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            };

            await blob_client.UploadAsync(stream, upload_options);

            return blob_client.Uri.ToString();
        }
    }
}
// end of BlobStorageService