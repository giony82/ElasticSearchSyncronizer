using System;
using Nest;

namespace Common.Elastic.Types
{
    public class DocumentBase
    {
        public string Id { get; set; }

        [Ignore]
        public bool Deleted { get; set; }

        [Ignore]
        public int NoOfRetry { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
