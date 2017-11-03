using System;

namespace AcumaticaESign
{
    public class AgreementCreationEntity
    {
        public string agreementId { get; set; }
        public string embeddedCode { get; set; }
        public DateTime expiration { get; set; }
        public string url { get; set; }
    }
}