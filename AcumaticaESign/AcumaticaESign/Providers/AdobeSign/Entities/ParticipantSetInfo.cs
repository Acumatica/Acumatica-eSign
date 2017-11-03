using System.Collections.Generic;

namespace AcumaticaESign
{
    public class ParticipantSetInfo
    {
        public string participantSetId { get; set; }
        public List<ParticipantInfo> participantSetMemberInfos { get; set; }
        public string roles { get; set; }
        public string status { get; set; }
        public string participantSetName { get; set; }
        public string privateMessage { get; set; }
        public int signingOrder { get; set; }
    }
}
