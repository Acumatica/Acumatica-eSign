namespace AcumaticaESign
{
    /// <summary>
    /// Contain base properties for crud operations of the<see cref="EnvelopeDocument"/> document.
    /// </summary>
    public class BaseRequestModel
    {
        public ESignAccount ESignAccount { get; set; }

        public string EnvelopeId { get; set; }
    }
}
