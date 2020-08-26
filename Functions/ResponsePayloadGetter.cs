using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Agstt.Functions
{
    public class ResponsePayloadGetter
    {
        private readonly ILogger<ResponsePayloadGetter> logger;

        public ResponsePayloadGetter(ILogger<ResponsePayloadGetter> logger)
        {
            this.logger = logger;
        }

        [FunctionName("ResponsePayloadGetter")]
        public HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{key}")] HttpRequest req,
            string key,
            [Blob("response-payloads/{key}", FileAccess.Read)] string payload)
        {
            if (payload == null)
            {
                logger.LogInformation($"Unable to find blob with Id: ({key}). Returning empty result...");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            logger.LogDebug($"Found blob with Id: ({key}). Returning...");
            return new HttpResponseMessage
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
        }
    }
}