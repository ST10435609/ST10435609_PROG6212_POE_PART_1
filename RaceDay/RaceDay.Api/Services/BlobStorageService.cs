// start of BlobStorageService
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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