using System;

namespace AcumaticaESign
{
    public class AdobeSignEnvelopeWithoutRecipientException : Exception
    {
        public AdobeSignEnvelopeWithoutRecipientException() : base(Messages.EnvelopeWithoutRecipientMessage)
        {
        }
    }
}
