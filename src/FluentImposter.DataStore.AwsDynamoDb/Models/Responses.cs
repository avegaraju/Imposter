using System;

namespace FluentImposter.DataStore.AwsDynamoDb.Models
{
    public class Responses
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public byte[] ResponsePayload { get; set; }
        public string ImposterName { get; set; }
        public string MatchedCondition { get; set; }
    }
}
