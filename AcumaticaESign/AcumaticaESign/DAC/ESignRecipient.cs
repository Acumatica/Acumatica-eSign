using System;
using PX.Data;
using PX.Objects.CR;

namespace AcumaticaESign
{
    /// <summary>
    /// Represents recipient who are will be sign <see cref="ESignEnvelopeInfo"/> envelope in ESign.
    /// Records of this type are created and edited through the ESign Document (ES 30.20.00) screen.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.ESignRecipient.CacheName)]
    public class ESignRecipient : IBqlTable
    {
        #region RecipientID
        public abstract class recipientID : IBqlField
        {
        }
        protected int? _RecipientID;

        /// <summary>
        /// Unique identifier of the recipient. Database identity.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
        public virtual int? RecipientID
        {
            get
            {
                return _RecipientID;
            }
            set
            {
                _RecipientID = value;
            }
        }
        #endregion
        #region EnvelopeInfoID
        public abstract class envelopeInfoID : IBqlField
        {
        }
        protected int? _EnvelopeInfoID;

        /// <summary>
        /// Identifier of the <see cref="ESignEnvelopeInfo"/>, whom the this recipient belongs.
        /// </summary>
        [PXDBInt]
        [PXDBDefault(typeof(ESignEnvelopeInfo.envelopeInfoID))]
        [PXParent(
            typeof(Select<ESignEnvelopeInfo,
                        Where<ESignEnvelopeInfo.envelopeInfoID,
                            Equal<Current<ESignRecipient.envelopeInfoID>>>>))]
        public virtual int? EnvelopeInfoID
        {
            get
            {
                return _EnvelopeInfoID;
            }
            set
            {
                _EnvelopeInfoID = value;
            }
        }
        #endregion
        #region Email
        public abstract class email : IBqlField
        {
        }
        protected string _Email;

        /// <summary>
        /// Email id of the recipient. Notification of the document to sign is sent to this email id.
        /// </summary>
        [PXDefault]
        [PXUIField(DisplayName = Messages.ESignRecipient.Email)]
        [PXDBEmail]
        [PXSelector(
            typeof(Search<Contact.eMail>), ValidateValue = false)]
        public virtual string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }
        #endregion
        #region Name
        public abstract class name : IBqlField
        {
        }
        protected string _Name;

        /// <summary>
        /// Legal name of the recipient.
        /// </summary>
        [PXDefault]
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignAccount.Name)]
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        #endregion
        #region Position
        public abstract class position : IBqlField
        {
        }
        protected int? _Position;

        /// <summary>
        /// Used for ordering recipients.
        /// </summary>
        [PXDBInt]
        [PXUIField(DisplayName = Messages.ESignRecipient.Position)]
        public virtual int? Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }
        #endregion
        #region CustomMessage
        public abstract class customMessage : IBqlField
        {
        }
        protected string _CustomMessage;

        /// <summary>
        /// Specifies the email body of the message sent to the recipient.
        /// </summary>
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignRecipient.CustomMessage)]
        public virtual string CustomMessage
        {
            get
            {
                return _CustomMessage;
            }
            set
            {
                _CustomMessage = value;
            }
        }
        #endregion
        #region Type
        public abstract class type : IBqlField
        {
        }
        protected string _Type;

        /// <summary>
        /// The type of the recipient.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"1"</c> - CopyRecipient,
        /// <c>"2"</c> - Signer.
        /// </value>
        [PXDBString(50, IsUnicode = true)]
        [PXDefault(RecipientTypes.Signer)]
        [PXStringList(new[]
        {
            RecipientTypes.CopyRecipient,
            RecipientTypes.Signer
        }, new[]
        {
            Messages.ESignRecipient.Type.CopyRecipient,
            Messages.ESignRecipient.Type.TypeSigner
        })]
        public virtual string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        #endregion
        #region Status
        public abstract class status : IBqlField
        {
        }
        protected string _Status;

        /// <summary>
        /// Indicates the envelope status. 
        /// </summary>
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignRecipient.Status)]
        [PXStringList(
             new[]
             {
                 RecipientStatus.New,
                 RecipientStatus.Created,
                 RecipientStatus.Completed,
                 RecipientStatus.Delivered,
                 RecipientStatus.NotYetVisible,
                 RecipientStatus.Sent,
                 RecipientStatus.WaitingForMySignature,
                 RecipientStatus.Declined,
                 RecipientStatus.Recalled,
                 RecipientStatus.Signed,
                 RecipientStatus.FaxPending,
                 RecipientStatus.AutoResponded,
                 RecipientStatus.Expired,
                 RecipientStatus.WaitingForMyApproval,
                 RecipientStatus.WaitingForMyAcceptance,
                 RecipientStatus.WaitingForMyAcknowledgement,
                 RecipientStatus.WaitingForMyFormFilling,
                 RecipientStatus.WaitingForMyDelegation,
                 RecipientStatus.OutForSignature,
                 RecipientStatus.Approved,
                 RecipientStatus.Accepted,
                 RecipientStatus.FormFilled,
                 RecipientStatus.Hidden,
                 RecipientStatus.WaitingForFaxin,
                 RecipientStatus.Archived,
                 RecipientStatus.Unknown,
                 RecipientStatus.Partial,
                 RecipientStatus.Form,
                 RecipientStatus.WaitingForAuthoring,
                 RecipientStatus.OutForApproval,
                 RecipientStatus.OutForAcceptance,
                 RecipientStatus.OutForDelivery,
                 RecipientStatus.OutForFormFilling,
                 RecipientStatus.Widget,
                 RecipientStatus.WaitingForMyReview,
                 RecipientStatus.InReview,
                 RecipientStatus.Other
             },
             new[]
             {
                 Messages.ESignRecipient.RecipientStatus.New,
                 Messages.ESignRecipient.RecipientStatus.Created,
                 Messages.ESignRecipient.RecipientStatus.Completed,
                 Messages.ESignRecipient.RecipientStatus.Delivered,
                 Messages.ESignRecipient.RecipientStatus.NotYetVisible,
                 Messages.ESignRecipient.RecipientStatus.Sent,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMySignature,
                 Messages.ESignRecipient.RecipientStatus.Declined,
                 Messages.ESignRecipient.RecipientStatus.Recalled,
                 Messages.ESignRecipient.RecipientStatus.Signed,
                 Messages.ESignRecipient.RecipientStatus.FaxPending,
                 Messages.ESignRecipient.RecipientStatus.AutoResponded,
                 Messages.ESignRecipient.RecipientStatus.Expired,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyApproval,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyAcceptance,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyAcknowledgement,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyFormFilling,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyDelegation,
                 Messages.ESignRecipient.RecipientStatus.OutForSignature,
                 Messages.ESignRecipient.RecipientStatus.Approved,
                 Messages.ESignRecipient.RecipientStatus.Accepted,
                 Messages.ESignRecipient.RecipientStatus.FormFilled,
                 Messages.ESignRecipient.RecipientStatus.Hidden,
                 Messages.ESignRecipient.RecipientStatus.WaitingForFaxin,
                 Messages.ESignRecipient.RecipientStatus.Archived,
                 Messages.ESignRecipient.RecipientStatus.Unknown,
                 Messages.ESignRecipient.RecipientStatus.Partial,
                 Messages.ESignRecipient.RecipientStatus.Form,
                 Messages.ESignRecipient.RecipientStatus.WaitingForAuthoring,
                 Messages.ESignRecipient.RecipientStatus.OutForApproval,
                 Messages.ESignRecipient.RecipientStatus.OutForAcceptance,
                 Messages.ESignRecipient.RecipientStatus.OutForDelivery,
                 Messages.ESignRecipient.RecipientStatus.OutForFormFilling,
                 Messages.ESignRecipient.RecipientStatus.Widget,
                 Messages.ESignRecipient.RecipientStatus.WaitingForMyReview,
                 Messages.ESignRecipient.RecipientStatus.InReview,
                 Messages.ESignRecipient.RecipientStatus.Other
             })]
        public virtual string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
        #endregion
        #region SignedDateTime
        public abstract class signedDateTime : IBqlField
        {
        }
        protected DateTime? _SignedDateTime;

        /// <summary>
        /// Date when envelope was signed by recipient.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.ESignRecipient.SignedDateTime)]
        public virtual DateTime? SignedDateTime
        {
            get
            {
                return _SignedDateTime;
            }
            set
            {
                _SignedDateTime = value;
            }
        }
        #endregion
        #region DeliveredDateTime
        public abstract class deliveredDateTime : IBqlField
        {
        }
        protected DateTime? _DeliveredDateTime;

        /// <summary>
        /// Date when envelope was delovered to recipient.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.ESignRecipient.DeliveredDateTime)]
        public virtual DateTime? DeliveredDateTime
        {
            get
            {
                return _DeliveredDateTime;
            }
            set
            {
                _DeliveredDateTime = value;
            }
        }
        #endregion
        #region tstamp
        public abstract class Tstamp : IBqlField
        {
        }
        protected byte[] _tstamp;
        [PXDBTimestamp]
        public virtual byte[] tstamp
        {
            get
            {
                return _tstamp;
            }
            set
            {
                _tstamp = value;
            }
        }
        #endregion
        #region CreatedByID
        public abstract class createdByID : IBqlField
        {
        }
        protected Guid? _CreatedByID;
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID
        {
            get
            {
                return _CreatedByID;
            }
            set
            {
                _CreatedByID = value;
            }
        }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : IBqlField
        {
        }
        protected string _CreatedByScreenID;
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
        {
            get
            {
                return _CreatedByScreenID;
            }
            set
            {
                _CreatedByScreenID = value;
            }
        }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : IBqlField
        {
        }
        protected DateTime? _CreatedDateTime;
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime
        {
            get
            {
                return _CreatedDateTime;
            }
            set
            {
                _CreatedDateTime = value;
            }
        }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : IBqlField
        {
        }
        protected Guid? _LastModifiedByID;
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID
        {
            get
            {
                return _LastModifiedByID;
            }
            set
            {
                _LastModifiedByID = value;
            }
        }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : IBqlField
        {
        }
        protected string _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
        {
            get
            {
                return _LastModifiedByScreenID;
            }
            set
            {
                _LastModifiedByScreenID = value;
            }
        }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : IBqlField
        {
        }
        protected DateTime? _LastModifiedDateTime;
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime
        {
            get
            {
                return _LastModifiedDateTime;
            }
            set
            {
                _LastModifiedDateTime = value;
            }
        }
        #endregion

        #region IPAddress

        public abstract class iPAddress : IBqlField
        {
        }

        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignRecipient.IpAddress)]
        public virtual string IPAddress
        {
            get;
            set;
        }

        #endregion

        public static class RecipientTypes
        {
            public const string Signer = "Signer";
            public const string CopyRecipient = "CopyRecipient";
        }

        public static class RecipientStatus
        {
            public const string New = "";
            public const string Created = "created";
            public const string Sent = "sent";
            public const string Declined = "declined";
            public const string Delivered = "delivered";
            public const string Signed = "signed";
            public const string Expired = "expired";
            public const string Completed = "completed";
            public const string FaxPending = "faxpending";
            public const string AutoResponded = "autoresponded";
            public const string WaitingForMySignature = "waiting_for_my_signature";
            public const string NotYetVisible = "not_yet_visible";
            public const string Recalled = "recalled";
            public const string WaitingForMyApproval = "waiting_for_my_approval";
            public const string WaitingForMyAcceptance = "waiting_for_my_acceptance";
            public const string WaitingForMyAcknowledgement = "waiting_for_my_acknowledgement";
            public const string WaitingForMyFormFilling = "waiting_for_my_form_filling";
            public const string WaitingForMyDelegation = "waiting_for_my_delegation";
            public const string OutForSignature = "out_for_signature";
            public const string Approved = "approved";
            public const string Accepted = "accepted";
            public const string FormFilled = "form_filled";
            public const string Hidden = "hidden";
            public const string WaitingForFaxin = "waiting_for_faxin";
            public const string Archived = "archived";
            public const string Unknown = "unknown";
            public const string Partial = "partial";
            public const string Form = "form";
            public const string WaitingForAuthoring = "waiting_for_authoring";
            public const string OutForApproval = "out_for_approval";
            public const string OutForAcceptance = "out_for_acceptance";
            public const string OutForDelivery = "out_for_delivery";
            public const string OutForFormFilling = "out_for_form_filling";
            public const string Widget = "widget";
            public const string WaitingForMyReview = "waiting_for_my_review";
            public const string InReview = "in_review";
            public const string Other = "other";
        }
    }
}