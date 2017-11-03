using System;

namespace AcumaticaESign
{
    public class AdobeSignDocumentWasDeletedException : Exception
    {
        public AdobeSignDocumentWasDeletedException() : base(Messages.DocumentWasDeletedMessage)
        {
        }
    }
}
