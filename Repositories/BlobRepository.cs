using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Agstt.Repositories
{
    public interface IBlobRepository
    {
        Task<BlobContentInfo> Add(string containerName, string blobKey, Stream blobContent);
    }

    public class BlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient serviceClient;
        private readonly ILogger<BlobRepository> logger;

        public BlobRepository(IConfiguration configuration, ILogger<BlobRepository> logger)
        {
            string connectionString = configuration.GetSection("AzureWebJobsStorage").Get<string>();
            logger.LogDebug($"Connection string: ({connectionString}).");
            this.serviceClient = new BlobServiceClient(connectionString);
            this.logger = logger;
        }

        // As it stands now the existence of container is checked on every operation. An argument can be made that it would be more effiecient to only perform this check once during initialization
        // of the repository (since it is a singleton). Should this be the chosen approach, containers could be created as mentioned and then cached by name.
        private async Task<BlobContainerClient> getContainerClient(string containerName)
        {
            var blobContainerClient = serviceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            return blobContainerClient;
        }

        public async Task<BlobContentInfo> Add(string containerName, string blobKey, Stream blobContent)
        {
            var containerClient = await getContainerClient(containerName);
            return await containerClient.UploadBlobAsync(blobKey, blobContent);
        }
    }
}