﻿using System;

namespace FluentImposter.Core.Models
{
    internal class Sessions
    {
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Status { get; set; }
    }
}
