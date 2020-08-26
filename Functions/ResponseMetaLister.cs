using Agstt.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Agstt.Functions
{
    public class ResponseMetaLister
    {
        private readonly IResponseMetaService responseMetasService;
        private readonly ILogger<ResponseMetaLister> logger;

        public ResponseMetaLister(IResponseMetaService responseMetasService, ILogger<ResponseMetaLister> logger)
        {
            this.responseMetasService = responseMetasService;
            this.logger = logger;
        }

        [FunctionName("ResponseMetaLister")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "meta-range")] HttpRequest req,
            [Table("ResponseMetas")] CloudTable table)
        {
            // Validate query parameters (range limits) and treat missing/invalid values as open bounds.
            if (!DateTimeOffset.TryParse(req.Query["from"], out var from))
            {
                from = DateTimeOffset.MinValue;
            }
            if (!DateTimeOffset.TryParse(req.Query["to"], out var to))
            {
                to = DateTimeOffset.MaxValue;
            }

            try
            {
                var result = await responseMetasService.GetDisplayablesRange(table, from, to);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while fetching response metas range. Aborting...");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
