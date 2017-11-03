using System;

namespace AcumaticaESign
{
    public class AdobeSignNotFoundException : Exception
    {
        public AdobeSignNotFoundException() : base(Messages.NotFoundMessage)
        {
        }
    }
}
