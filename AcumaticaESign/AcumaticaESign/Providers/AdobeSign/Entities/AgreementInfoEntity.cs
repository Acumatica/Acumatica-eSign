using System;
using System.Collections.Generic;

namespace AcumaticaESign
{
    public class AgreementInfoEntity
    {
        public string agreementId { get; set; }
        public List<DocumentHistoryEvent> events { get; set; }
        public string latestVersionId { get; set; }
        public string locale { get; set; }
        public bool modifiable { get; set; }
        public string name { get; set; }
        public List<NextParticipantSetInfo> nextParticipantSetInfos { get; set; }
        public List<ParticipantSetInfo> participantSetInfos { get; set; }
        public string signatureType { get; set; }
        public string status { get; set; }
        public string vaultingEnabled { get; set; }
        public string message { get; set; }
        public DateTime expiration { get; set; }
    }
}
