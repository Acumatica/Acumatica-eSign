using System;
using PX.Data;
using PX.TM;

namespace AcumaticaESign
{
    /// <summary>
    /// Represents the mapping rule to <see cref="ESignAccount"> ESign account</see>
    /// and Acumatica user.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.ESignAccountUserRule.CacheName)]
    public class ESignAccountUserRule : IBqlTable
    {
        #region AccountID

        public abstract class accountID : IBqlField
        {
        }

        /// <summary>
        /// Unique identifier of the ESign account.
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(ESignAccount.accountID))]
        [PXParent(typeof(Select<ESignAccount,
            Where<ESignAccount.accountID, Equal<Current<accountID>>>>))]
        public virtual int? AccountID
        {
            get;
            set;
        }

        #endregion

        #region OwnerID

        public abstract class ownerID : IBqlField
        {
        }

        /// <summary>
        /// Default acumatica user related to ESign account.
        /// </summary>
        [PXDBGuid(IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXOwnerSelector]
        [PXUIField(DisplayName = Messages.ESignAccountUserRule.AccountOwner)]
        public virtual Guid? OwnerID
        {
            get;
            set;
        }

        #endregion

        #region tstamp

        public abstract class Tstamp : IBqlField
        {
        }

        [PXDBTimestamp]
        public virtual byte[] tstamp
        {
            get;
            set;
        }

        #endregion

        #region CreatedByID

        public abstract class createdByID : IBqlField
        {
        }

        [PXDBCreatedByID]
        public virtual Guid? CreatedByID
        {
            get;
            set;
        }

        #endregion

        #region CreatedByScreenID

        public abstract class createdByScreenID : IBqlField
        {
        }

        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
        {
            get;
            set;
        }

        #endregion

        #region CreatedDateTime

        public abstract class createdDateTime : IBqlField
        {
        }

        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime
        {
            get;
            set;
        }

        #endregion

        #region LastModifiedByID

        public abstract class lastModifiedByID : IBqlField
        {
        }

        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID
        {
            get;
            set;
        }

        #endregion

        #region LastModifiedByScreenID

        public abstract class lastModifiedByScreenID : IBqlField
        {
        }

        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
        {
            get;
            set;
        }

        #endregion

        #region LastModifiedDateTime

        public abstract class lastModifiedDateTime : IBqlField
        {
        }

        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime
        {
            get;
            set;
        }

        #endregion
    }
}
