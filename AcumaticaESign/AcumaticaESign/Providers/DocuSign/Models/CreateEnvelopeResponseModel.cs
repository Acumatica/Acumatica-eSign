using DocuSign.eSign.Model;

namespace AcumaticaESign
{
    /// <summary>
    /// Represent response from the creating new envelope
    /// request in <see cref="EnvelopesApi"/> rest api.
    /// </summary>
    public class CreateEnvelopeResponseModel
    {
        public ViewUrl ViewUrl { get; set; }

        public EnvelopeSummary EnvelopeSummary { get; set; }
    }
}
