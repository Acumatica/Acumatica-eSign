using System;
using PX.Data;
using PX.TM;

namespace AcumaticaESign
{
    /// <summary>
    /// Represents envelope <see cref="ESignEnvelopeInfo"/> owner filter for 
    /// ESign Central (ES 40.10.00) screen.
    /// </summary>
    [Serializable]
    public class EnvelopeFilter : IBqlTable
    {
        #region MyOwner
        public abstract class myOwner : IBqlField
        {
        }
        protected bool? _MyOwner;

        /// <summary>
        /// Defined if rows should be filtered with current owner.
        /// </summary>
        [PXBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = Messages.EnvelopeFilter.Me)]
        public virtual bool? MyOwner
        {
            get
            {
                return _MyOwner;
            }
            set
            {
                _MyOwner = value;
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : IBqlField
        {
        }
        protected Guid? _OwnerID;

        /// <summary>
        /// Defined owner parameter of the filter.
        /// </summary>
        [PXDBGuid]
        [PXUIField(DisplayName = Messages.EnvelopeFilter.Owner, Enabled = false)]
        [PXOwnerSelector]
        public virtual Guid? OwnerID
        {
            get
            {
                return _MyOwner.HasValue && _MyOwner.Value
                    ? CurrentOwnerID
                    : _OwnerID;
            }
            set
            {
                _OwnerID = value;
            }
        }
        #endregion
        #region CurrentOwnerID
        public abstract class currentOwnerID : IBqlField
        {
        }
        protected Guid? _CurrentOwnerID;

        /// <summary>
        /// Defined current owner.
        /// </summary>
        [PXGuid]
        [PXDefault(typeof(AccessInfo.userID))]
        public virtual Guid? CurrentOwnerID
        {
            get
            {
                return _CurrentOwnerID;
            }
            set
            {
                _CurrentOwnerID = value;
            }
        }
        #endregion
    }
}