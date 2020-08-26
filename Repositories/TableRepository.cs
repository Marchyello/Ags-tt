using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Agstt.Repositories
{
    public interface ITableRepository
    {
        Task<TableQuerySegment<TEntity>> GetRange<TEntity>(CloudTable table, string queryFilter = null) where TEntity : TableEntity, new();
        Task<TableResult> Add<TEntity>(CloudTable table, TEntity entity) where TEntity : TableEntity;
    }

    public class TableRepository : ITableRepository
    {
        private readonly ILogger<TableRepository> logger;

        public TableRepository(ILogger<TableRepository> logger)
        {
            this.logger = logger;
        }

        public async Task<TableQuerySegment<TEntity>> GetRange<TEntity>(CloudTable table, string queryFilter = null) where TEntity : TableEntity, new()
        {
            logger.LogDebug($"Preparing to fetch range from table: ({table.Name})...");
            var query = new TableQuery<TEntity>();
            if (queryFilter != null)
            {
                logger.LogDebug($"Query filter applied: ({queryFilter}).");
                query = query.Where(queryFilter);
            }

            return await table.ExecuteQuerySegmentedAsync(query, null);
        }

        public async Task<TableResult> Add<TEntity>(CloudTable table, TEntity entity) where TEntity : TableEntity
        {
            logger.LogDebug($"Adding entity with row key: ({entity.RowKey}) to table: ({table.Name})...");
            var insertOperation = TableOperation.Insert(entity);

            return await table.ExecuteAsync(insertOperation);
        }
    }
}