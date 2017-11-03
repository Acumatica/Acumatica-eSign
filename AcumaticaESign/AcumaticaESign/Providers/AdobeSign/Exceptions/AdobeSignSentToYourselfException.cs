using System;

namespace AcumaticaESign
{
    public class AdobeSignSentToYourselfException : Exception
    {
        public AdobeSignSentToYourselfException() : base(Messages.SentToYourselfMessage)
        {
        }
    }
}
