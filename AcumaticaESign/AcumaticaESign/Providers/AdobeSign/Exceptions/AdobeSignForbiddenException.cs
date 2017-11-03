using System;

namespace AcumaticaESign
{
    public class AdobeSignForbiddenException : Exception
    {
        public AdobeSignForbiddenException() : base(Messages.AccessIsNotAllowedMessage)
        {
        }
    }
}
