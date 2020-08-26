using Agstt.Entities;
using Agstt.Models;
using Agstt.Repositories;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agstt.Services
{
    public interface IResponseMetaService
    {
        Task<IEnumerable<ResponseMetaDisplayable>> GetDisplayablesRange(CloudTable responseMetasTable, DateTimeOffset from, DateTimeOffset to);
        Task<string> Add(CloudTable responseMetasTable, HttpResponseMessage response);
    }

    public class ResponseMetaService : IResponseMetaService
    {
        private readonly ITableRepository tableRepository;
        private readonly ILogger<ResponseMetaService> logger;

        public ResponseMetaService(ITableRepository tableRepository, ILogger<ResponseMetaService> logger)
        {
            this.tableRepository = tableRepository;
            this.logger = logger;
        }

        public async Task<IEnumerable<ResponseMetaDisplayable>> GetDisplayablesRange(CloudTable responseMetasTable, DateTimeOffset from, DateTimeOffset to)
        {
            logger.LogDebug($"Looking up response metas within time limits from: ({from}) to: ({to})...");

            // Both bounds are inclusive.
            string lowerBoundFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, from.ToString(ResponseMeta.KeyFormat));
            string upperBoundFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, to.ToString(ResponseMeta.KeyFormat));
            string queryFilter = TableQuery.CombineFilters(lowerBoundFilter, TableOperators.And, upperBoundFilter);

            return (await tableRepository
                .GetRange<ResponseMeta>(responseMetasTable, queryFilter))
                .Select(rm => new ResponseMetaDisplayable(rm));
        }

        public async Task<string> Add(CloudTable responseMetasTable, HttpResponseMessage response)
        {
            var responseMeta = new ResponseMeta(response, DateTimeOffset.UtcNow);
            logger.LogDebug($"Adding new response meta:\n{responseMeta}");
            await tableRepository.Add(responseMetasTable, responseMeta);
            return responseMeta.RowKey;
        }
    }
}