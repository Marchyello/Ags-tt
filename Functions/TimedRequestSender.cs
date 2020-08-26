using Agstt.Services;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agstt.Functions
{
    public class TimedRequestSender
    {
        private const string requestUrl = "https://api.publicapis.org/random?auth=null";
        private readonly HttpClient httpClient;
        private readonly IResponseMetaService responseMetaService;
        private readonly IResponsePayloadService responsePayloadService;
        private readonly ILogger<TimedRequestSender> logger;

        public TimedRequestSender(
            HttpClient httpClient,
            IResponseMetaService responseMetaService,
            IResponsePayloadService responsePayloadService,
            ILogger<TimedRequestSender> logger)
        {
            this.httpClient = httpClient;
            this.responseMetaService = responseMetaService;
            this.responsePayloadService = responsePayloadService;
            this.logger = logger;
        }

        [FunctionName("TimedRequestSender")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("ResponseMetas")] CloudTable table)
        {
            logger.LogInformation($"Executing TimedRequestSender function at: {DateTime.UtcNow}...");

            HttpResponseMessage response = null;
            try
            {
                response = await httpClient.GetAsync(requestUrl);
            }
            // This is not a terminating error, we proceed with saving attempt as a failure.
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Failure while fetching from ({requestUrl}).");
            }

            try
            {
                string key = await responseMetaService.Add(table, response);
                if (response != null)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    await responsePayloadService.Add(key, responseStream);
                }
                logger.LogInformation($"TimedRequestSender function successfully finised executing at: {DateTime.UtcNow}...");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving response to storage.");
            }
        }
    }
}
