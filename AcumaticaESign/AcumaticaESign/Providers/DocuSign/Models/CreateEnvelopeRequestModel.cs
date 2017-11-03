using System.Collections.Generic;
using PX.SM;

namespace AcumaticaESign
{
    /// <summary>
    /// Contain properties which are needed for creating new request of 
    /// the <see cref="ESignEnvelopeInfo"/> envelope.
    /// </summary>
    public class CreateEnvelopeRequestModel: BaseRequestModel
    {
        public FileInfo FileInfo { get; set; }

        public ESignEnvelopeInfo EnvelopeInfo { get; set; }

        public List<ESignRecipient> Recipients { get; set; }

        public List<ESignRecipient> CarbonRecipients { get; set; }
    }
}
