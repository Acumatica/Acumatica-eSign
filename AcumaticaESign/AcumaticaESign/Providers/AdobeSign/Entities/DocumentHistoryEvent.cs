using System;

namespace AcumaticaESign
{
    public class DocumentHistoryEvent
    {
        public string actingUserEmail { get; set; }
        public string actingUserIpAddress { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
        public string participantEmail { get; set; }
        public string type { get; set; }
        public string versionId { get; set; }
    }
}