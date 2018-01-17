using System;

namespace FluentImposter.DataStore.AwsDynamoDb.Models
{
    public class Sessions
    {
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Status { get; set; }
    }
}
