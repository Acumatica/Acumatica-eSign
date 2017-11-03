using DocuSign.eSign.Model;

namespace AcumaticaESign
{
    /// <summary>
    /// Contain properties which are needed for creating new request 
    /// to get history of the <see cref="ESignEnvelopeInfo"/> envelope.
    /// </summary>
    public class GetEnvelopeHistoryResponseModel
    {
        public Envelope Envelope { get; set; }

        public Recipients Recipients { get; set; }

        public Notification Notification { get; set; }

        public EnvelopeAuditEventResponse Events { get; set; }
    }
}
