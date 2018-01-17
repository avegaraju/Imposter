using System;

using ServiceStack.DataAnnotations;

namespace FluentImposter.DataStore.AwsDynamoDb.Models
{
    public class FI_SESSIONS
    {
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Status { get; set; }
    }
}
