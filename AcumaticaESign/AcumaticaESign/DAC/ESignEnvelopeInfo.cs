using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.SM;

namespace AcumaticaESign
{
    [Serializable]
    [PXCacheName(Messages.ESignEnvelopeInfo.CacheName)]
    public class ESignEnvelopeInfo : IBqlTable
    {
        #region EnvelopeInfoID

        public abstract class envelopeInfoID : IBqlField
        {
        }

        /// <summary>
        /// Unique identifier of the envelope info. Database identity.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
        public virtual int? EnvelopeInfoID
        {
            get;
            set;
        }

        #endregion

        #region FileID

        public abstract class fileID : IBqlField
        {
        }

        /// <summary>
        /// The indetifier of the file which will be send to ESign.
        /// </summary>
        [PXDBGuid]
        [PXDBDefault(typeof(UploadFileRevision.fileID))]
        [PXParent(typeof(Select<UploadFileRevision, Where<UploadFileRevision.fileID, Equal<Current<fileID>>>>))]
        public virtual Guid? FileID
        {
            get;
            set;
        }

        #endregion

        #region FileRevisionID

        public abstract class fileRevisionID : IBqlField
        {
        }

        /// <summary>
        /// The indetifier of the file revision.
        /// </summary>
        [PXDBInt]
        [PXDBDefault(typeof(UploadFileRevision.fileRevisionID))]
        [PXParent(typeof(Select<UploadFileRevision, Where<UploadFileRevision.fileRevisionID, Equal<Current<fileRevisionID>>>>))]
        public virtual int? FileRevisionID
        {
            get;
            set;
        }

        #endregion

        #region EnvelopeID

        public abstract class envelopeID : IBqlField
        {
        }

        /// <summary>
        /// The unique indetifier of the ESign envelope.
        /// Store value which came from ESign service.
        /// </summary>
        [PXDBString(255, IsUnicode = true)]
        public virtual string EnvelopeID
        {
            get; set;

        }

        #endregion

        #region ReviewUrl

        public abstract class reviewUrl : IBqlField
        {
        }

        [PXDBString(255, IsUnicode = true)]
        public virtual string ReviewUrl
        {
            get;
            set;
        }

        #endregion

        #region LastStatus
        public abstract class lastStatus : IBqlField
        {
        }

        /// <summary>
        /// The latest status of the envelope.
        /// </summary>
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.LastStatus)]
        [PXStringList(
             new[]
             {
                 EnvelopeStatus.New,

                 EnvelopeStatus.DocuSign.Created,
                 EnvelopeStatus.DocuSign.Completed,
                 EnvelopeStatus.DocuSign.Delivered,
                 EnvelopeStatus.DocuSign.Sent,
                 EnvelopeStatus.DocuSign.Declined,
                 EnvelopeStatus.DocuSign.Signed,
                 EnvelopeStatus.DocuSign.Voided,
                 EnvelopeStatus.DocuSign.Deleted,

                 EnvelopeStatus.AdobeSign.OutForSignature,
                 EnvelopeStatus.AdobeSign.WaitingForReview,
                 EnvelopeStatus.AdobeSign.Signed,
                 EnvelopeStatus.AdobeSign.Approved,
                 EnvelopeStatus.AdobeSign.Accepted,
                 EnvelopeStatus.AdobeSign.Delivered,
                 EnvelopeStatus.AdobeSign.FormFilled,
                 EnvelopeStatus.AdobeSign.Aborted,
                 EnvelopeStatus.AdobeSign.DocumentLibrary,
                 EnvelopeStatus.AdobeSign.Widget,
                 EnvelopeStatus.AdobeSign.Expired,
                 EnvelopeStatus.AdobeSign.Archived,
                 EnvelopeStatus.AdobeSign.Prefill,
                 EnvelopeStatus.AdobeSign.Authoring,
                 EnvelopeStatus.AdobeSign.WaitingForFaxin,
                 EnvelopeStatus.AdobeSign.WaitingForVerification,
                 EnvelopeStatus.AdobeSign.WaitingForPayment,
                 EnvelopeStatus.AdobeSign.OutForApproval,
                 EnvelopeStatus.AdobeSign.OutForAcceptance,
                 EnvelopeStatus.AdobeSign.OutForDelivery,
                 EnvelopeStatus.AdobeSign.OutForFromFilling,
                 EnvelopeStatus.AdobeSign.Other
             },
             new[]
             {
                 Messages.ESignEnvelopeStatus.New,

                 Messages.ESignEnvelopeStatus.Created,
                 Messages.ESignEnvelopeStatus.Completed,
                 Messages.ESignEnvelopeStatus.Delivered,
                 Messages.ESignEnvelopeStatus.Sent,
                 Messages.ESignEnvelopeStatus.Declined,
                 Messages.ESignEnvelopeStatus.Signed,
                 Messages.ESignEnvelopeStatus.Voided,
                 Messages.ESignEnvelopeStatus.Deleted,

                 Messages.ESignEnvelopeStatus.OutForSignature,
                 Messages.ESignEnvelopeStatus.WaitingForReview,
                 Messages.ESignEnvelopeStatus.Signed,
                 Messages.ESignEnvelopeStatus.Approved,
                 Messages.ESignEnvelopeStatus.Accepted,
                 Messages.ESignEnvelopeStatus.Delivered,
                 Messages.ESignEnvelopeStatus.FormFilled,
                 Messages.ESignEnvelopeStatus.Aborted,
                 Messages.ESignEnvelopeStatus.DocumentLibrary,
                 Messages.ESignEnvelopeStatus.Widget,
                 Messages.ESignEnvelopeStatus.Expired,
                 Messages.ESignEnvelopeStatus.Archived,
                 Messages.ESignEnvelopeStatus.Prefill,
                 Messages.ESignEnvelopeStatus.Authoring,
                 Messages.ESignEnvelopeStatus.WaitingForFaxin,
                 Messages.ESignEnvelopeStatus.WaitingForVerification,
                 Messages.ESignEnvelopeStatus.WaitingForPayment,
                 Messages.ESignEnvelopeStatus.OutForApproval,
                 Messages.ESignEnvelopeStatus.OutForAcceptance,
                 Messages.ESignEnvelopeStatus.OutForDelivery,
                 Messages.ESignEnvelopeStatus.OutForFromFilling,
                 Messages.ESignEnvelopeStatus.Other
             })]
        public virtual string LastStatus
        {
            get;
            set;
        }

        #endregion

        #region ActivityDate
        public abstract class activityDate : IBqlField
        {
        }

        /// <summary>
        /// The date of the item was last modified.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.ActivityDate)]
        public virtual DateTime? ActivityDate
        {
            get;
            set;
        }

        #endregion

        #region ESignAccountID

        public abstract class eSignAccountID : IBqlField
        {
        }

        /// <summary>
        /// Identifier of the <see cref="ESignAccount"/>, whom the this envelope belongs.
        /// </summary>
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.Account)]
        [PXSelector(typeof(Search2<ESignAccount.accountID,
            LeftJoin<ESignAccountUserRule, On<ESignAccountUserRule.accountID, Equal<ESignAccount.accountID>>>,
            Where2<Where2<Where<ESignAccount.ownerID, IsNotNull,
                        And<ESignAccount.ownerID, Equal<Current<AccessInfo.userID>>>>,
                    Or<Where<ESignAccountUserRule.ownerID, IsNotNull,
                        And<ESignAccountUserRule.ownerID, Equal<Current<AccessInfo.userID>>>>>>,
                And<ESignAccount.isActive, Equal<True>>>,
            OrderBy<Asc<ESignAccount.type, Asc<AccessInfo.userName>>>>),
            new Type[]
            {
                typeof(ESignAccount.accountCD),
                typeof(ESignAccount.type),
                typeof(ESignAccount.providerType)
            },
            SubstituteKey = typeof(ESignAccount.accountCD))]
        public virtual int? ESignAccountID
        {
            get;
            set;
        }

        #endregion

        #region Theme

        public abstract class theme : IBqlField
        {
        }

        /// <summary>
        /// Specifies the subject of the email that is sent to all recipients.
        /// </summary>
        [PXDBString(100, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.Theme)]
        public virtual string Theme
        {
            get;
            set;
        }

        #endregion

        #region MessageBody

        public abstract class messageBody : IBqlField
        {
        }

        /// <summary>
        /// Specifies the body of the email that is sent to all recipients.
        /// </summary>
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.MessageBody)]
        public virtual string MessageBody
        {
            get;
            set;
        }

        #endregion

        #region IsOrder

        public abstract class isOrder : IBqlField
        {
        }

        /// <summary>
        /// Specifies if it is needed to use ordering of the <see cref="ESignRecipient"/>
        /// recipients in current envelope.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.IsOrder)]
        public virtual bool? IsOrder
        {
            get;
            set;
        }

        #endregion

        #region Reminder Type

        public abstract class reminderType : IBqlField
        {
        }

        [PXDBString(50, IsFixed = true)]
        [PXDefault(typeof(Search<ESignAccount.reminderType,
                Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.RemindersType, Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new[]
        {
            Messages.ESignReminderType.DailyValue,
            Messages.ESignReminderType.WeeklyValue
        }, new[]
        {
            Messages.ESignReminderType.DailyLabel,
            Messages.ESignReminderType.WeeklyLabel
        })]
        public virtual string ReminderType
        {
            get;
            set;
        }

        #endregion

        #region SendReminders
        public abstract class sendReminders : IBqlField
        {
        }

        /// <summary>
        /// Defindes if it is needed to send reminders to <see cref="ESignRecipient">Recipients</see>.
        /// Default value is retrieved from <see cref="ESignAccount">Account</see>.
        /// </summary>
        [PXDBBool]
        [PXDefault(typeof(Search<ESignAccount.sendReminders, Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.SendReminders)]
        public virtual bool? SendReminders
        {
            get;
            set;
        }

        #endregion

        #region ExpiredDays
        public abstract class expiredDays : IBqlField
        {
        }

        /// <summary>
        /// An integer that sets the number of days the envelope is active.
        /// Default value is retrieved from <see cref="ESignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<ESignAccount.expiredDays, Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.ExpiredDays)]
        public virtual int? ExpiredDays
        {
            get;
            set;
        }
        #endregion

        #region WarnDays

        public abstract class warnDays : IBqlField
        {
        }

        /// <summary>
        /// An integer that sets the number of days before envelope expiration that an expiration 
        /// warning email is sent to the recipient. If set to 0 (zero), no warning email is sent.
        /// Default value is retrieved from <see cref="ESignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<ESignAccount.warnDays, Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.WarnDays)]
        public virtual int? WarnDays
        {
            get;
            set;
        }

        #endregion

        #region FirstReminderDay

        public abstract class firstReminderDay : IBqlField
        {
        }

        /// <summary>
        /// An interger that sets the number of days after the recipient receives the envelope 
        /// that reminder emails are sent to the recipient.
        /// Default value is retrieved from <see cref="ESignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<ESignAccount.firstReminderDay, Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.FirstReminderDay)]
        public virtual int? FirstReminderDay
        {
            get;
            set;
        }

        #endregion

        #region ReminderFrequency

        public abstract class reminderFrequency : IBqlField
        {
        }

        /// <summary>
        /// An interger that sets the interval, in days, between reminder emails.
        /// Default value is retrieved from <see cref="ESignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<ESignAccount.reminderFrequency, Where<ESignAccount.accountID, Equal<Current<eSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.ReminderFrequency)]
        public virtual int? ReminderFrequency
        {
            get;
            set;
        }

        #endregion

        #region Selected

        public abstract class selected : IBqlField
        {
        }

        /// <summary>
        /// Defines if current envelope is selected.
        /// </summary>
        [PXBool]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.IsSelected)]
        public virtual bool? Selected
        {
            get;
            set;
        }

        #endregion

        #region IsFinalVersion

        public abstract class isFinalVersion : IBqlField
        {
        }

        /// <summary>
        /// Defines if current envelope is final version of the signed document.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        public virtual bool? IsFinalVersion
        {
            get;
            set;
        }

        #endregion

        #region CompletedFileID

        public abstract class completedFileID : IBqlField
        {
        }

        /// <summary>
        /// The identifier of the uploaded final version of the signed document.
        /// </summary>
        [PXDBGuid]
        public virtual Guid? CompletedFileID
        {
            get;
            set;
        }

        #endregion

        #region CompletedFileName

        public abstract class completedFileName : IBqlField
        {
        }

        /// <summary>
        /// The name of the uploaded final version of the signed document.
        /// </summary>
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.CompletedFileName)]
        public virtual string CompletedFileName
        {
            get;
            set;
        }

        #endregion

        #region IsActionsAvailable

        public abstract class isActionsAvailable : IBqlField
        {
        }

        /// <summary>
        /// Defines if actions (remind or void) are availiable for current envelope.
        /// </summary>
        [PXBool]
        [PXUIField(Enabled = false, Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual bool? IsActionsAvailable
        {
            get
            {
                var listStatus = new List<string>
                {
                    EnvelopeStatus.New,
                    EnvelopeStatus.DocuSign.Created,
                    EnvelopeStatus.DocuSign.Completed,
                    EnvelopeStatus.DocuSign.Voided,
                    EnvelopeStatus.DocuSign.Declined,
                    EnvelopeStatus.DocuSign.Deleted,
                    EnvelopeStatus.AdobeSign.Aborted,
                    EnvelopeStatus.AdobeSign.Authoring,
                    EnvelopeStatus.AdobeSign.WaitingForReview,
                    EnvelopeStatus.AdobeSign.Signed,
                    EnvelopeStatus.AdobeSign.Archived
                };
                return listStatus.All(x => !listStatus.Contains(LastStatus));
            }
        }

        #endregion

        #region IsDeleteAvailable

        public abstract class isDeleteAvailable : IBqlField
        {
        }

        /// <summary>
        /// Defines if actions (remind or void) are availiable for current envelope.
        /// </summary>
        [PXBool]
        [PXUIField(Enabled = false, Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual bool? IsDeleteAvailable
        {
            get
            {
                var listStatus = new List<string>
                {
                    EnvelopeStatus.New,
                    EnvelopeStatus.DocuSign.Created,
                    EnvelopeStatus.AdobeSign.Authoring
                };
                return listStatus.Any(x => listStatus.Contains(LastStatus));
            }
        }

        #endregion

        #region SendDate

        public abstract class sendDate : IBqlField
        {
        }

        /// <summary>
        /// The Date when current envelope was send to ESign.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.SendDate)]
        public virtual DateTime? SendDate
        {
            get;
            set;
        }

        #endregion

        #region ExpirationDate

        public abstract class expirationDate : IBqlField
        {
        }

        /// <summary>
        /// The Date when current envelope expired on ESign.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.ExpirationDate)]
        public virtual DateTime? ExpirationDate
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

        [PXDBCreatedByID(Visible = false)]
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
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.CreatedDate)]
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

        [PXDBLastModifiedByID(Visible = false)]
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

        #region AcumaticaEnvelopeStatus

        public abstract class acumaticaEnvelopeStatus : IBqlField
        {
        }

        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.LastStatus)]
        [PXStringList(new[]
        {
            Messages.ESignEnvelopeStatus.New,
            Messages.ESignEnvelopeStatus.Signed,
            Messages.ESignEnvelopeStatus.Delivered,

            Messages.ESignEnvelopeStatus.Created,
            Messages.ESignEnvelopeStatus.Completed,
            Messages.ESignEnvelopeStatus.Sent,
            Messages.ESignEnvelopeStatus.Declined,
            Messages.ESignEnvelopeStatus.Voided,
            Messages.ESignEnvelopeStatus.Deleted,

            Messages.ESignEnvelopeStatus.OutForESignature,
            Messages.ESignEnvelopeStatus.WaitingForReview,
            Messages.ESignEnvelopeStatus.Approved,
            Messages.ESignEnvelopeStatus.Accepted,
            Messages.ESignEnvelopeStatus.FormFilled,
            Messages.ESignEnvelopeStatus.CancelledDeclined,
            Messages.ESignEnvelopeStatus.DocumentLibrary,
            Messages.ESignEnvelopeStatus.Widget,
            Messages.ESignEnvelopeStatus.Expired,
            Messages.ESignEnvelopeStatus.Archived,
            Messages.ESignEnvelopeStatus.WaitingForMeToPrefill,
            Messages.ESignEnvelopeStatus.Draft,
            Messages.ESignEnvelopeStatus.WaitingForFaxin,
            Messages.ESignEnvelopeStatus.WaitingForVerification,
            Messages.ESignEnvelopeStatus.WaitingForPayment,
            Messages.ESignEnvelopeStatus.OutForApproval,
            Messages.ESignEnvelopeStatus.OutForAcceptance,
            Messages.ESignEnvelopeStatus.OutForDelivery,
            Messages.ESignEnvelopeStatus.OutForFromFilling,
            Messages.ESignEnvelopeStatus.Other
        }, new[]
        {
            Messages.ESignEnvelopeStatus.New,
            Messages.ESignEnvelopeStatus.Signed,
            Messages.ESignEnvelopeStatus.Delivered,

            Messages.ESignEnvelopeStatus.Created,
            Messages.ESignEnvelopeStatus.Completed,
            Messages.ESignEnvelopeStatus.Sent,
            Messages.ESignEnvelopeStatus.Declined,
            Messages.ESignEnvelopeStatus.Voided,
            Messages.ESignEnvelopeStatus.Deleted,

            Messages.ESignEnvelopeStatus.OutForESignature,
            Messages.ESignEnvelopeStatus.WaitingForReview,
            Messages.ESignEnvelopeStatus.Approved,
            Messages.ESignEnvelopeStatus.Accepted,
            Messages.ESignEnvelopeStatus.FormFilled,
            Messages.ESignEnvelopeStatus.CancelledDeclined,
            Messages.ESignEnvelopeStatus.DocumentLibrary,
            Messages.ESignEnvelopeStatus.Widget,
            Messages.ESignEnvelopeStatus.Expired,
            Messages.ESignEnvelopeStatus.Archived,
            Messages.ESignEnvelopeStatus.WaitingForMeToPrefill,
            Messages.ESignEnvelopeStatus.Draft,
            Messages.ESignEnvelopeStatus.WaitingForFaxin,
            Messages.ESignEnvelopeStatus.WaitingForVerification,
            Messages.ESignEnvelopeStatus.WaitingForPayment,
            Messages.ESignEnvelopeStatus.OutForApproval,
            Messages.ESignEnvelopeStatus.OutForAcceptance,
            Messages.ESignEnvelopeStatus.OutForDelivery,
            Messages.ESignEnvelopeStatus.OutForFromFilling,
            Messages.ESignEnvelopeStatus.Other
        })]
        public virtual string AcumaticaEnvelopeStatus
        {
            get
            {
                switch (LastStatus)
                {
                    case null:
                    case EnvelopeStatus.New:
                        return Messages.ESignEnvelopeStatus.New;
                    case EnvelopeStatus.DocuSign.Signed:
                    case EnvelopeStatus.AdobeSign.Signed:
                        return Messages.ESignEnvelopeStatus.Signed;
                    case EnvelopeStatus.DocuSign.Delivered:
                    case EnvelopeStatus.AdobeSign.Delivered:
                        return Messages.ESignEnvelopeStatus.Delivered;

                    case EnvelopeStatus.DocuSign.Created:
                        return Messages.ESignEnvelopeStatus.Created;
                    case EnvelopeStatus.DocuSign.Completed:
                        return Messages.ESignEnvelopeStatus.Completed;
                    case EnvelopeStatus.DocuSign.Sent:
                        return Messages.ESignEnvelopeStatus.Sent;
                    case EnvelopeStatus.DocuSign.Declined:
                        return Messages.ESignEnvelopeStatus.Declined;
                    case EnvelopeStatus.DocuSign.Voided:
                        return Messages.ESignEnvelopeStatus.Voided;
                    case EnvelopeStatus.DocuSign.Deleted:
                        return Messages.ESignEnvelopeStatus.Deleted;

                    case EnvelopeStatus.AdobeSign.OutForSignature:
                        return Messages.ESignEnvelopeStatus.OutForESignature;
                    case EnvelopeStatus.AdobeSign.WaitingForReview:
                        return Messages.ESignEnvelopeStatus.WaitingForReview;
                    case EnvelopeStatus.AdobeSign.Approved:
                        return Messages.ESignEnvelopeStatus.Approved;
                    case EnvelopeStatus.AdobeSign.Accepted:
                        return Messages.ESignEnvelopeStatus.Accepted;
                    case EnvelopeStatus.AdobeSign.FormFilled:
                        return Messages.ESignEnvelopeStatus.FormFilled;
                    case EnvelopeStatus.AdobeSign.Aborted:
                        return Messages.ESignEnvelopeStatus.CancelledDeclined;
                    case EnvelopeStatus.AdobeSign.DocumentLibrary:
                        return Messages.ESignEnvelopeStatus.DocumentLibrary;
                    case EnvelopeStatus.AdobeSign.Widget:
                        return Messages.ESignEnvelopeStatus.Widget;
                    case EnvelopeStatus.AdobeSign.Expired:
                        return Messages.ESignEnvelopeStatus.Expired;
                    case EnvelopeStatus.AdobeSign.Archived:
                        return Messages.ESignEnvelopeStatus.Archived;
                    case EnvelopeStatus.AdobeSign.Prefill:
                        return Messages.ESignEnvelopeStatus.WaitingForMeToPrefill;
                    case EnvelopeStatus.AdobeSign.Authoring:
                        return Messages.ESignEnvelopeStatus.Draft;
                    case EnvelopeStatus.AdobeSign.WaitingForFaxin:
                        return Messages.ESignEnvelopeStatus.WaitingForFaxin;
                    case EnvelopeStatus.AdobeSign.WaitingForVerification:
                        return Messages.ESignEnvelopeStatus.WaitingForVerification;
                    case EnvelopeStatus.AdobeSign.WaitingForPayment:
                        return Messages.ESignEnvelopeStatus.WaitingForPayment;
                    case EnvelopeStatus.AdobeSign.OutForApproval:
                        return Messages.ESignEnvelopeStatus.OutForApproval;
                    case EnvelopeStatus.AdobeSign.OutForAcceptance:
                        return Messages.ESignEnvelopeStatus.OutForAcceptance;
                    case EnvelopeStatus.AdobeSign.OutForDelivery:
                        return Messages.ESignEnvelopeStatus.OutForDelivery;
                    case EnvelopeStatus.AdobeSign.OutForFromFilling:
                        return Messages.ESignEnvelopeStatus.OutForFromFilling;
                    case EnvelopeStatus.AdobeSign.Other:
                        return Messages.ESignEnvelopeStatus.Other;
                    default:
                        return LastStatus;
                }
            }
        }

        #endregion

        #region ProviderType

        public abstract class providerType : IBqlField
        {
        }

        [PXString(1)]
        [PXUIField(DisplayName = Messages.ESignEnvelopeInfo.ProviderType, Visibility = PXUIVisibility.SelectorVisible,
            Enabled = false)]
        [PXDBScalar(typeof(Search<ESignAccount.providerType,
            Where<ESignAccount.accountID, Equal<eSignAccountID>>>))]
        [PXStringList(new[]
        {
            ESignAccount.ProviderTypes.DocuSign,
            ESignAccount.ProviderTypes.AdobeSign
        }, new[]
        {
            Messages.ESignProviderType.DocuSign,
            Messages.ESignProviderType.AdobeSign
        })]
        public virtual string ProviderType
        {
            get;
            set;
        }

        #endregion

        #region Provider Type Icon

        [PXUIField(DisplayName = Messages.ESignAccount.ProviderTypeIcon, Enabled = false)]
        [PXFormula(typeof (
            Switch<
                Case<Where<providerType, Equal<docuSginProvider>>,
                    docuSignPath,
                    Case<Where<providerType, Equal<adobeSignProvider>>,
                        adobeSignPath>>,
                emptyPath>))]
        public virtual string ProviderTypeIcon
        {
            get;
            set;
        }

        #endregion
    }

    public static class ProviderTypes
    {
        public const string DocuSign = "D";
        public const string AdobeSign = "A";
        public const string Empty = "";
    }

    public static class ProviderTypesIconPath
    {
        public const string DocuSign = "~/Icons/DocuSignIcon.png";
        public const string AdobeSign = "~/Icons/AdobeSignIcon.png";
        public const string Empty = "";
    }

    public class docuSginProvider : Constant<string>
    {
        public docuSginProvider() : base(ProviderTypes.DocuSign)
        {
        }
    }

    public class adobeSignProvider : Constant<string>
    {
        public adobeSignProvider() : base(ProviderTypes.AdobeSign)
        {
        }
    }

    public class emptyProvider : Constant<string>
    {
        public emptyProvider() : base(ProviderTypes.Empty)
        {
        }
    }

    public class docuSignPath : Constant<string>
    {
        public docuSignPath() : base(ProviderTypesIconPath.DocuSign)
        {
        }
    }

    public class adobeSignPath : Constant<string>
    {
        public adobeSignPath() : base(ProviderTypesIconPath.AdobeSign)
        {
        }
    }

    public class emptyPath : Constant<string>
    {
        public emptyPath() : base(ProviderTypesIconPath.Empty)
        {
        }
    }

    public class envelopeStatusCompleted : Constant<string>
    {
        public envelopeStatusCompleted() : base(EnvelopeStatus.DocuSign.Completed) { }
    }

    public class envelopeStatusVoided : Constant<string>
    {
        public envelopeStatusVoided() : base(EnvelopeStatus.DocuSign.Voided) { }
    }

    public class envelopeStatusSigned : Constant<string>
    {
        public envelopeStatusSigned() : base(EnvelopeStatus.AdobeSign.Signed) { }
    }

    public class envelopeStatusDeclined : Constant<string>
    {
        public envelopeStatusDeclined() : base(EnvelopeStatus.AdobeSign.Aborted) { }
    }

    public static class EnvelopeStatus
    {
        public const string New = "";

        public static class DocuSign
        {
            public const string Created = "created";
            public const string Sent = "sent";
            public const string Declined = "declined";
            public const string Delivered = "delivered";
            public const string Signed = "signed";
            public const string Completed = "completed";
            public const string Voided = "voided";
            public const string Deleted = "deleted";
        }

        public static class AdobeSign
        {
            public const string OutForSignature = "OUT_FOR_SIGNATURE";
            public const string WaitingForReview = "WAITING_FOR_REVIEW";
            public const string Signed = "SIGNED";
            public const string Approved = "APPROVED";
            public const string Accepted = "ACCEPTED";
            public const string Delivered = "DELIVERED";
            public const string FormFilled = "FORM_FILLED";
            public const string Aborted = "ABORTED";
            public const string DocumentLibrary = "DOCUMENT_LIBRARY";
            public const string Widget = "WIDGET";
            public const string Expired = "EXPIRED";
            public const string Archived = "ARCHIVED";
            public const string Prefill = "PREFILL";
            public const string Authoring = "AUTHORING";
            public const string WaitingForFaxin = "WAITING_FOR_FAXIN";
            public const string WaitingForVerification = "WAITING_FOR_VERIFICATION";
            public const string WaitingForPayment = "WAITING_FOR_PAYMENT";
            public const string OutForApproval = "OUT_FOR_APPROVAL";
            public const string OutForAcceptance = "OUT_FOR_ACCEPTANCE";
            public const string OutForDelivery = "OUT_FOR_DELIVERY";
            public const string OutForFromFilling = "OUT_FOR_FORM_FILLING";
            public const string Other = "OTHER";
        }
    }
}
