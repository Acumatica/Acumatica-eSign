using PX.Common;

namespace AcumaticaESign
{
    [PXLocalizable(Prefix)]
    public static class Messages
    {
        public const string SiteMapTitleDisplayName = "Source Screen Name";
        public const string DefaultEnvelopeVoidReason = "Document voided.";

        public static class ESignAccount
        {
            public const string CacheName = "eSign Account";
            public const string Email = "Email";
            public const string Name = "Name";
            public const string AccountOwner = "Acumatica User Name";
            public const string ClientId = "Client ID";
            public const string ClientSecret = "Client Secret";
            public const string ApiUrl = "API URL";
            public const string Status = "Status";
            public const string AccountCd = "eSign Account";
            public const string RemindersType = "Reminders Frequency Type";
            public const string AccountType = "Type";
            public const string ProviderType = "Provider Type";
            public const string ProviderTypeIcon = "";
            public const string SendReminders = "Send Automatic Reminders";
            public const string ExpiredDays = "# of days before request expires";
            public const string WarnDays = "# of days to warn before expiration";
            public const string FirstReminderDay = "# of days before sending 1st reminder";
            public const string ReminderFrequency = "# of days between reminders";
            public const string Password = "Password";
            public const string IsTestApi = "Use Test API";
            public const string IsActive = "Active";
        }

        public static class ESignRecipient
        {
            public const string CacheName = "eSign Recipient";
            public const string Email = "Email";
            public const string Position = "Position";
            public const string CustomMessage = "Custom Message";
            public const string Status = "Recipient Status";
            public const string SignedDateTime = "Signed Date";
            public const string DeliveredDateTime = "Delivered Date";
            public const string IpAddress = "IP Address";

            public static class Type
            {
                public const string TypeSigner = "Needs to Sign";
                public const string CopyRecipient = "Receives a Copy";
            }

            public static class RecipientStatus
            {
                public const string New = "";
                public const string Created = "Created";
                public const string Sent = "Sent";
                public const string Declined = "Declined";
                public const string Recalled = "Recalled";
                public const string Delivered = "Delivered";
                public const string Signed = "Signed";
                public const string Completed = "Completed";
                public const string FaxPending = "Fax Pending";
                public const string NotYetVisible = "Not yet visible";
                public const string WaitingForMySignature = "Waiting for my signature";
                public const string AutoResponded = "Auto Responded";
                public const string Expired = "Expired";
                public const string WaitingForMyApproval = "Waiting for my approval";
                public const string WaitingForMyAcceptance = "Waiting for my acceptance";
                public const string WaitingForMyAcknowledgement = "Waiting for my acknowledgement";
                public const string WaitingForMyFormFilling = "Waiting for my form filling";
                public const string WaitingForMyDelegation = "Waiting for my delegation";
                public const string OutForSignature = "Out for signature";
                public const string Approved = "Approved";
                public const string Accepted = "Accepted";
                public const string FormFilled = "Form filled";
                public const string Hidden = "Hidden";
                public const string WaitingForFaxin = "Waiting for faxin";
                public const string Archived = "Archived";
                public const string Unknown = "Unknown";
                public const string Partial = "Partial";
                public const string Form = "Form";
                public const string WaitingForAuthoring = "Waiting for authoring";
                public const string OutForApproval = "Out for approval";
                public const string OutForAcceptance = "Out for acceptance";
                public const string OutForDelivery = "Out for delivery";
                public const string OutForFormFilling = "Out for form filling";
                public const string Widget = "Widget";
                public const string WaitingForMyReview = "Waiting for my review";
                public const string InReview = "In review";
                public const string Other = "Other";
            }
        }

        public static class ESignEnvelopeInfo
        {
            public const string CacheName = "eSign Envelope";
            public const string SendReminders = "Send Automatic Reminders";
            public const string ExpiredDays = "# of days before request expires";
            public const string WarnDays = "# of days to warn before expiration";
            public const string FirstReminderDay = "# of days before sending 1st reminder";
            public const string ReminderFrequency = "# of days between reminders";
            public const string RemindersType = "Reminders Frequency Type";
            public const string LastStatus = "eSign Status";
            public const string ProviderType = "Provider Type";
            public const string ActivityDate = "eSign Activity Date";
            public const string AccountOwner = "eSign Owner";
            public const string CreatedDate = "Creation Date";
            public const string SendDate = "eSign Send Date";
            public const string ExpirationDate = "Expiration Date";
            public const string Theme = "Subject";
            public const string MessageBody = "Message Body";
            public const string IsSelected = "Selected";
            public const string IsOrder = "Set Signing Order";
            public const string CompletedFileName = "eSign Version";
            public const string Account = "eSign Account";
        }

        public static class VoidRequestFilter
        {
            public const string VoidReason = "Void Reason";
        }

        public static class EnvelopeFilter
        {
            public const string Me = "Me";
            public const string Owner = "Owner";
        }

        public static class ESignAccountUserRule
        {
            public const string CacheName = "eSign Account User Rule";
            public const string AccountOwner = "Acumatica User Name";
        }

        public static class Actions
        {
            public const string Connect = "Connect";
            public const string Disconnect = "Disconnect";
        }

        public static class ESignIntegrationStatus
        {
            public const string Connected = "Connected";
            public const string Disconnected = "Disconnected";
        }

        public static class ESignProviderType
        {
            public const string DocuSign = "DocuSign";
            public const string AdobeSign = "AdobeSign";
        }

        public static class ESignAccountType
        {
            public const string Individual = "Individual";
            public const string Shared = "Shared";
        }

        public static class ESignReminderType
        {
            public const string DailyValue = "DAILY_UNTIL_SIGNED";
            public const string WeeklyValue = "WEEKLY_UNTIL_SIGNED";
            public const string DailyLabel = "Daily";
            public const string WeeklyLabel = "Weekly";
        }

        public static class ESignEnvelopeStatus
        {
            public const string Created = "Created";
            public const string Sent = "Sent";
            public const string Declined = "Declined";
            public const string Completed = "Completed";
            public const string Voided = "Voided";
            public const string Deleted = "Deleted";

            public const string OutForSignature = "Out For Signature";
            public const string WaitingForReview = "Waiting For Review";
            public const string Approved = "Approved";
            public const string Accepted = "Accepted";
            public const string FormFilled = "Form Filled";
            public const string Aborted = "Aborted";
            public const string DocumentLibrary = "Document Library";
            public const string Widget = "Widget";
            public const string Expired = "Expired";
            public const string Archived = "Archived";
            public const string Prefill = "Prefill";
            public const string Authoring = "Authoring";
            public const string WaitingForFaxin = "Waiting For Fax";
            public const string WaitingForVerification = "Waiting For Verification";
            public const string WaitingForPayment = "Waiting For Payment";
            public const string OutForApproval = "Out For Approval";
            public const string OutForAcceptance = "Out For Acceptance";
            public const string OutForDelivery = "Out For Delivery";
            public const string OutForFromFilling = "Out For From Filling";
            public const string Other = "Other";

            public const string New = "";
            public const string Delivered = "Delivered";
            public const string Signed = "Signed";

            public const string WaitingForMeToPrefill = "Waiting For Me to Prefill";
            public const string Draft = "Draft";
            public const string OutForESignature = "Out for e-signature";
            public const string CancelledDeclined = "Cancelled/Declined";
        }

        #region Custom Actions

        public const string TestConnectionFlow = "Test Connection";
        public const string ESignSelected = "ESign";
        public const string Void = "Void Document";
        public const string Remind = "Remind Recipient";
        public const string ViewESign = "View Document";
        public const string ViewHistory = "View History";
        public const string ViewFile = "View File";
        public const string CheckStatus = "Check Status";
        public const string ESignEnvelopeSend = "Send";
        public const string ESignEnvelopeSave = "Save";
        public const string ESignEnvelopeSaveAndClose = "Save & Close";
        public const string Ok = "Ok";

        #endregion

        #region Validation messages

        public const string EnvelopeStatusChanged =
            "This document was already sent previously. Please refresh your screen to see latest status information.";

        public const string ESignAccountNotExists =
            "eSign Account is not setup for logged-in user. Please contact your System administrator.";

        public const string ESignEnvelopeDeleteConfirm =
            "Are you sure you want to delete and void this document from eSign?";

        public const string EnvelopeVoidIsNotAvailable = "Status was changed. Void action is not available.";
        public const string EnvelopeRemindIsNotAvailable = "Status was changed. Remind action is not available.";
        public const string ESignRecipientsRequired = "At least one recipient is required.";
        public const string ESignRecipientsUnique = "Each recipient in the same signing order must be unique.";
        public const string ESignEnvelopeNotExists = "This Document is not available in eSign.";
        public const string ESignEnvelopeLock = "This Document is locked by eSign until {0}. Please try later.";
        public const string Prefix = "Acumatica E Sign Error.";
        public const string SharedAccountExists = "Shared account already exists";
        public const string ESignEnvelopeDeleteFileConfirm = "Current File record will be deleted";
        public const string ESignAccountSharedNotNullUsers = "At least one Acumatica user should be specified";
        public const string ESignEnvelopeLockNotExist = "EDIT_LOCK_ENVELOPE_NOT_LOCKED";
        public const string FieldRequired = "Error: '{0}' cannot be empty.";
        public const string ProviderTypeIsMissing = "Provider Type is missing";
        public const string ESignAccountIdIsMissing = "eSign Account is missing.";

        public const string DocumentWasDeletedMessage = "Document was deleted from web version.";
        public const string EnvelopeWithoutRecipientMessage = "You cannot send a document without recipient.";
        public const string MessagePattern = "Unsupported values ({0}) have been sent to Adobe Sign.";
        public const string AccessIsNotAllowedMessage = "Access is not allowed.";
        public const string InvalidFileTypeMessage = "Invalid file type chosen for E Sign.";
        public const string NotFoundMessage = "Adobe Sign entity has not been found.";
        public const string SentToYourselfMessage = "You cannot send a document only to yourself to complete.";

        #endregion
    }
}
