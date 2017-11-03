using System;
using PX.Data;
using PX.TM;

namespace AcumaticaESign
{
    [Serializable]
    [PXCacheName(Messages.ESignAccount.CacheName)]
    public class ESignAccount : IBqlTable
    {
        #region AccountID

        public abstract class accountID : IBqlField
        {
        }

        /// <summary>
        /// Unique identifier of the account. Database identity.
        /// </summary>
        [PXDBIdentity]
        public virtual int? AccountID
        {
            get;
            set;
        }

        #endregion

        #region AccountCD

        public abstract class accountCD : IBqlField
        {
        }

        /// <summary>
        /// Key field.
        /// The user-friendly unique identifier of the account.
        /// </summary>
        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = Messages.ESignAccount.AccountCd, Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<accountCD>),
            new Type[]
            {
                typeof(accountCD),
                typeof(type),
                typeof(providerType)
            })]
        public virtual string AccountCD
        {
            get;
            set;
        }

        #endregion

        #region Account Type

        public abstract class type : IBqlField
        {
        }

        /// <summary>
        /// The type of the account.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"I"</c> - Individual,
        /// <c>"S"</c> - Shared.
        /// </value>
        [PXDBString(1, IsFixed = true)]
        [PXDefault(AccountTypes.Individual)]
        [PXUIField(DisplayName = Messages.ESignAccount.AccountType, Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new[]
            {
                AccountTypes.Individual,
                AccountTypes.Shared
            },
            new[]
            {
                Messages.ESignAccountType.Individual,
                Messages.ESignAccountType.Shared
            })]
        public virtual string Type
        {
            get;
            set;
        }

        #endregion

        #region IsActive

        public abstract class isActive : IBqlField
        {
        }

        /// <summary>
        /// Indicates whether the Account is active.
        /// </summary>
        [PXDBBool]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = Messages.ESignAccount.IsActive)]
        public virtual bool? IsActive
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
        /// Default acumatica user related to esign account.
        /// </summary>
        [PXDBGuid]
        [PXDefault]
        [PXOwnerSelector]
        [PXUIField(DisplayName = Messages.ESignAccount.AccountOwner)]
        public virtual Guid? OwnerID
        {
            get;
            set;
        }

        #endregion

        #region Client ID

        public abstract class clientID : IBqlField
        {
        }

        [PXRSACryptString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignAccount.ClientId, Visibility = PXUIVisibility.Visible)]
        public virtual string ClientID
        {
            get;
            set;
        }

        #endregion

        #region Client Secret

        public abstract class clientSecret : IBqlField
        {
        }

        [PXRSACryptString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignAccount.ClientSecret, Visibility = PXUIVisibility.Visible)]
        public virtual string ClientSecret
        {
            get;
            set;
        }

        #endregion

        #region Api Url

        public abstract class apiUrl : IBqlField
        {
        }

        [PXDefault]
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignAccount.ApiUrl, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string ApiUrl
        {
            get;
            set;
        }

        public abstract class apiAccessPoint : IBqlField
        {
        }

        [PXDBString(255, IsUnicode = true)]
        public virtual string ApiAccessPoint
        {
            get;
            set;
        }

        #endregion

        #region Access Token

        public abstract class accessToken : IBqlField
        {
        }

        [PXRSACryptString(255, IsUnicode = true)]
        public virtual string AccessToken
        {
            get;
            set;
        }

        #endregion

        #region Refresh Token

        public abstract class refreshToken : IBqlField
        {
        }

        [PXRSACryptString(255, IsUnicode = true)]
        public virtual string RefreshToken
        {
            get;
            set;
        }

        #endregion

        #region Status

        public abstract class status : IBqlField
        {
        }

        [PXDBString(55, IsUnicode = true)]
        [PXDefault(Messages.ESignIntegrationStatus.Disconnected)]
        [PXUIField(DisplayName = Messages.ESignAccount.Status, Visibility = PXUIVisibility.SelectorVisible,
            Enabled = false)]
        [PXStringList(new[]
            {
                Messages.ESignIntegrationStatus.Connected,
                Messages.ESignIntegrationStatus.Disconnected
            },
            new[]
            {
                Messages.ESignIntegrationStatus.Connected,
                Messages.ESignIntegrationStatus.Disconnected
            })]
        public virtual string Status
        {
            get;
            set;
        }

        #endregion

        #region Provider Type
        public abstract class providerType : IBqlField
        {
        }

        [PXDBString(1, IsFixed = true)]
        [PXDefault(ProviderTypes.AdobeSign)]
        [PXUIField(DisplayName = Messages.ESignAccount.ProviderType, Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new[]
        {
            ProviderTypes.DocuSign,
            ProviderTypes.AdobeSign
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
        public abstract class providerTypeIcon : IBqlField
        {
        }

        [PXUIField(DisplayName = Messages.ESignAccount.ProviderTypeIcon, Enabled = false)]
        [PXFormula(typeof(
            Switch<
                Case<Where<providerType, Equal<docuSginProvider>>,
                    docuSignPath,
                Case<Where<providerType, Equal<adobeSignProvider>>,
                    adobeSignPath>>,
                emptyPath>))]
        public virtual string ProviderTypeIcon { get; set; }

        #endregion

        #region SendReminders

        public abstract class sendReminders : IBqlField
        {
        }

        /// <summary>
        /// Defindes if it is needed to send reminders to <see cref="ESignRecipient">Recipients</see>.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = Messages.ESignAccount.SendReminders)]
        public virtual bool? SendReminders
        {
            get;
            set;
        }

        #endregion

        #region Reminder Type

        public abstract class reminderType : IBqlField
        {
        }

        [PXDBString(50, IsUnicode = true, IsFixed = false)]
        [PXDefault(Messages.ESignReminderType.DailyValue, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = Messages.ESignAccount.RemindersType, Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new[]
            {
                Messages.ESignReminderType.DailyValue,
                Messages.ESignReminderType.WeeklyValue
            },
            new[]
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

        #region ExpiredDays

        public abstract class expiredDays : IBqlField
        {
        }

        /// <summary>
        /// An integer that sets the number of days the envelope is active.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(120)]
        [PXUIField(DisplayName = Messages.ESignAccount.ExpiredDays)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
        [PXUIField(DisplayName = Messages.ESignAccount.WarnDays)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
        [PXUIField(DisplayName = Messages.ESignAccount.FirstReminderDay)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
        [PXUIField(DisplayName = Messages.ESignAccount.ReminderFrequency)]
        public virtual int? ReminderFrequency
        {
            get;
            set;
        }

        #endregion

        #region IsTestApi
        public abstract class isTestApi : IBqlField
        {
        }

        /// <summary>
        /// Defined if it is needed to use test api.
        /// </summary>
        [PXDBBool]
        [PXUIField(DisplayName = Messages.ESignAccount.IsTestApi)]
        public virtual bool? IsTestApi
        {
            get;
            set;
        }
        #endregion

        #region Email
        public abstract class email : IBqlField
        {
        }

        /// <summary>
        /// Email address of the account.
        /// </summary>
        [PXDBEmail]
        [PXUIField(DisplayName = Messages.ESignAccount.Email, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Email
        {
            get;
            set;
        }
        #endregion

        #region Password
        public abstract class password : IBqlField
        {
        }

        /// <summary>
        /// Account password.
        /// </summary>
        [PXRSACryptString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.ESignAccount.Password)]
        public virtual string Password
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

        public static class AccountTypes
        {
            public const string Shared = "S";
            public const string Individual = "I";
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

        public class connected : Constant<string>
        {
            public connected() : base(Messages.ESignIntegrationStatus.Connected)
            {
            }
        }

        public class disconnected : Constant<string>
        {
            public disconnected() : base(Messages.ESignIntegrationStatus.Disconnected)
            {
            }
        }
    }
}
