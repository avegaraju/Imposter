﻿using System;

namespace FluentImposter.DataStore.AwsDynamoDb.Models
{
    public class Requests
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Resource { get; set; }
        public string HttpMethod { get; set; }
        public byte[] RequestPayload { get; set; }
    }
}
