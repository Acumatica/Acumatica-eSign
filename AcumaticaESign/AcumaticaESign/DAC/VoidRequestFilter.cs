using System;
using PX.Data;

namespace AcumaticaESign
{
    [Serializable]
    public class VoidRequestFilter : IBqlTable
    {
        #region VoidReason

        public abstract class voidReason : IBqlField
        {
        }

        /// <summary>
        /// Specifies the void reason for the <see cref="ESignEnvelopeInfo"/> envelope.
        /// </summary>
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.VoidRequestFilter.VoidReason)]
        public virtual string VoidReason
        {
            get;
            set;
        }

        #endregion
    }
}
