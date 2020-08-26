using Agstt.Repositories;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Agstt.Services
{
    public interface IResponsePayloadService
    {
        Task Add(string blobKey, Stream blobContent);
    }

    public class ResponsePayloadService : IResponsePayloadService
    {
        private const string containerName = "response-payloads";
        private readonly IBlobRepository blobRepository;
        private readonly ILogger<ResponsePayloadService> logger;

        public ResponsePayloadService(IBlobRepository blobRepository, ILogger<ResponsePayloadService> logger)
        {
            this.blobRepository = blobRepository;
            this.logger = logger;
        }

        public async Task Add(string blobKey, Stream blobContent) => await blobRepository.Add(containerName, blobKey, blobContent);
    }
}