using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Net.Http;

namespace Agstt.Entities
{
    public class ResponseMeta : TableEntity
    {
        public const string KeyFormat = "yyyy-MM-ddTHH:mm";

        public ResponseMeta() { }

        public ResponseMeta(HttpResponseMessage response, DateTimeOffset receivedOn)
        {
            // Truncate precision (seconds and above) to avoid query misses due to logically insignificant latency, i.e.,
            // query upper bound value 2020-08-24T09:20:00.000 misses an entry with observed value 2020-08-24T09:20:00.012.
            RowKey = receivedOn.ToString(KeyFormat);
            PartitionKey = response != null
                ? response.IsSuccessStatusCode.ToString()
                : false.ToString();

            if (response != null)
            {
                StatusCode = (int)response.StatusCode;
                Headers = string.Join('\n', response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)};"));
            }
        }

        // Receipt timestamp and status are already stored as row/partition keys, so skip them here to avoid redundancy.
        public int? StatusCode { get; set; }
        public string Headers { get; set; }

        public override string ToString() =>
$@"RowKey: {RowKey};
PartitionKey: {PartitionKey};
StatusCode: {StatusCode};
Headers: {Headers};";
    }
}