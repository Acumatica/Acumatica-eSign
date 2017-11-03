using System;

namespace AcumaticaESign
{
    public class AdobeSignUnauthorizedException : Exception
    {
        public AdobeSignUnauthorizedException(string message = null) : base(message)
        {
        }
    }
}
