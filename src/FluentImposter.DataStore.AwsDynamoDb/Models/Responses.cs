using System;

namespace FluentImposter.DataStore.AwsDynamoDb.Models
{
    public class Responses
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string ResponsePayloadBase64 { get; set; }
        public string ImposterName { get; set; }
        public string MatchedCondition { get; set; }
    }
}
