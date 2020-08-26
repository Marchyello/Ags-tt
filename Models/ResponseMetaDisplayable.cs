using Agstt.Entities;
using System;

namespace Agstt.Models
{
    public class ResponseMetaDisplayable
    {
        public string BlobKey { get; set; }
        public DateTimeOffset ReceivedOn { get; set; }
        public bool IsSuccess { get; set; }
        public int? StatusCode { get; set; }
        public string Headers { get; set; }

        public ResponseMetaDisplayable(ResponseMeta responseMeta)
        {
            if (responseMeta == null)
            {
                throw new ArgumentNullException("responseMeta");
            }

            BlobKey = responseMeta.RowKey;
            ReceivedOn = DateTimeOffset.Parse(responseMeta.RowKey);
            IsSuccess = bool.Parse(responseMeta.PartitionKey);
            StatusCode = responseMeta.StatusCode;
            Headers = responseMeta.Headers;
        }
    }
}