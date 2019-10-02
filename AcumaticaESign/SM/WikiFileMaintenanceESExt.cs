using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using CsvHelper;
using DocuSign.eSign.Model;
using PX.Common;
using PX.CS.Contracts.Interfaces;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.SM;
using PX.Web.UI;
using FileInfo = PX.SM.FileInfo;
using Note = PX.Data.Note;

namespace AcumaticaESign.SM
{
    /// <summary>
    /// Extends graph <see cref="WikiFileMaintenance"/> for envelopes functionality.
    /// Records of this type are created and edited through the Wiki File Maintenance (SM.20.25.10) screen.
    /// </summary>
    public class WikiFileMaintenanceESExt : PXGraphExtension<WikiFileMaintenance>
    {

        private const string DocuSignProvider = ProviderTypes.DocuSign;
        private const string AdobeSignProvider = ProviderTypes.AdobeSign;

        #region Selects

        public PXSelectJoin<UploadFileRevisionNoData,
            LeftJoin<ESignEnvelopeInfo, On<ESignEnvelopeInfo.fileID, Equal<UploadFileRevisionNoData.fileID>,
                And<ESignEnvelopeInfo.fileRevisionID, Equal<UploadFileRevisionNoData.fileRevisionID>>>>,
            Where<UploadFileRevisionNoData.fileID, Equal<Current<UploadFileWithIDSelector.fileID>>>> RevisionsWithAction;

        public PXSelect<Contact> Contacts;

        public PXSelectJoin<ESignEnvelopeInfo,
            InnerJoin<ESignAccount, On<ESignAccount.accountID, Equal<ESignEnvelopeInfo.eSignAccountID>>>,
            Where<ESignEnvelopeInfo.fileID, Equal<Current<UploadFileRevisionNoData.fileID>>,
                And<ESignEnvelopeInfo.fileRevisionID, Equal<Current<UploadFileRevisionNoData.fileRevisionID>>>>> Envelope;

        public PXSelect<ESignRecipient,
            Where<ESignRecipient.envelopeInfoID, Equal<Current<ESignEnvelopeInfo.envelopeInfoID>>>,
            OrderBy<Asc<ESignRecipient.position>>> Recipients;

        public PXSelectJoin<ESignAccount,
            LeftJoin<ESignAccountUserRule, On<ESignAccountUserRule.accountID, Equal<ESignAccount.accountID>>>,
            Where<ESignAccount.isActive, Equal<True>,
                And2<Where<ESignAccount.ownerID, IsNotNull,
                        And<ESignAccount.ownerID, Equal<Current<AccessInfo.userID>>>>,
                    Or<Where<ESignAccountUserRule.ownerID, IsNotNull,
                        And<ESignAccountUserRule.ownerID, Equal<Current<AccessInfo.userID>>>>>>>,
            OrderBy<Asc<ESignAccount.type, Asc<AccessInfo.userName>>>> ESignAccount;

        #endregion

        #region Events
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName", Messages.ESignEnvelopeInfo.AccountOwner)]
        protected virtual void ESignEnvelopeInfo_ESignAccountID_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName",
            Messages.ESignEnvelopeInfo.CompletedFileName)]
        protected void UploadFileWithIDSelector_Name_CacheAttached(PXCache sender)
        {
        }

        protected virtual void ESignEnvelopeInfo_RowUpdating(PXCache sender,
                                                 PXRowUpdatingEventArgs e)
        {
            ESignEnvelopeInfo row = (ESignEnvelopeInfo)e.NewRow;
            if (row.ESignAccountID == null)
            {
                sender.RaiseExceptionHandling<ESignEnvelopeInfo.eSignAccountID>(
                    row, null,
                    new PXSetPropertyException(
                        Messages.ESignAccountIdIsMissing,
                        typeof(ESignEnvelopeInfo.eSignAccountID).Name));
                e.Cancel = true;
            }
            else if (((ESignEnvelopeInfo)e.Row).ESignAccountID != row.ESignAccountID)
            {
                var account = new PXSelect<ESignAccount,
                       Where<ESignAccount.accountID, Equal<Required<ESignAccount.accountID>>>>(Base)
                   .SelectSingle(row.ESignAccountID);
                row.FirstReminderDay = account.FirstReminderDay;
                row.ReminderFrequency = account.ReminderFrequency;
                row.ExpiredDays = account.ExpiredDays;
                row.ReminderType = account.ReminderType;
                row.SendReminders = account.SendReminders;
                row.WarnDays = account.WarnDays;
            }
        }

        protected virtual void _(Events.RowSelected<ESignEnvelopeInfo> args)
        {
            var envelope = (ESignEnvelopeInfo)args.Row;
            if (envelope == null)
            {
                return;
            }

            PXUIFieldAttribute.SetVisible<ESignRecipient.position>(Recipients.Cache, null, envelope.IsOrder == true);
            var isAdobe = envelope.ProviderType == ProviderTypes.AdobeSign;

            PXUIFieldAttribute.SetVisible<ESignEnvelopeInfo.reminderType>(Envelope.Cache, null, isAdobe);
            PXUIFieldAttribute.SetVisible<ESignEnvelopeInfo.warnDays>(Envelope.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignEnvelopeInfo.firstReminderDay>(Envelope.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignEnvelopeInfo.reminderFrequency>(Envelope.Cache, null, !isAdobe);

            PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.firstReminderDay>(Envelope.Cache, null, envelope.SendReminders == true);
            PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.reminderFrequency>(Envelope.Cache, null, envelope.SendReminders == true);
            PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.reminderType>(Envelope.Cache, null, envelope.SendReminders == true);

            if (envelope.LastStatus != EnvelopeStatus.New && envelope.LastStatus != null)
            {
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.firstReminderDay>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.reminderFrequency>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.reminderType>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.expiredDays>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.warnDays>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.theme>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.messageBody>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.sendReminders>(Envelope.Cache, null, false);
            }


            if (envelope.LastStatus != EnvelopeStatus.New && envelope.LastStatus != EnvelopeStatus.DocuSign.Created && envelope.LastStatus != null)
            {
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.eSignAccountID>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<ESignEnvelopeInfo.isOrder>(Envelope.Cache, null, false);

                Recipients.AllowUpdate = false;
                Recipients.AllowDelete = false;
                Recipients.AllowInsert = false;
            }

            UpdateRecipientsGridLayout(envelope.ProviderType);
        }

        protected virtual void _(Events.FieldUpdated<ESignEnvelopeInfo.eSignAccountID> args)
        {
            var envelope = args.Row as ESignEnvelopeInfo;
            if (envelope?.ESignAccountID == null)
            {
                return;
            }

            var account = new PXSelect<ESignAccount,
                    Where<ESignAccount.accountID, Equal<Required<ESignEnvelopeInfo.eSignAccountID>>>>(Base)
                .SelectSingle(envelope.ESignAccountID);
            envelope.SendReminders = account.SendReminders;
            envelope.ReminderType = account.ReminderType;
            envelope.ProviderType = account.ProviderType;

            UpdateRecipientsGridLayout(account.ProviderType);
        }

        protected virtual void _(Events.FieldUpdated<ESignRecipient.email> args)
        {
            var doc = (ESignRecipient)args.Row;
            if (doc != null)
            {
                Contact contact = Contacts.Search<Contact.eMail>(doc.Email);
                if (!string.IsNullOrEmpty(contact?.DisplayName))
                {
                    doc.Name = contact.DisplayName;
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<ESignEnvelopeInfo.isOrder> args)
        {
            var env = (ESignEnvelopeInfo)args.Row;
            if (env?.IsOrder != null && !env.IsOrder.Value && Envelope.Current != null)
            {
                var recipients = Recipients.Select();
                foreach (ESignRecipient recipient in recipients)
                {
                    recipient.Position = null;
                }
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Override default refresh action on grid.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> Refresh;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageKey = Sprite.Main.Refresh)]
        public virtual IEnumerable refresh(PXAdapter adapter)
        {
            var fileId = Base.Files.Current.FileID;

            CleanCache();

            PXLongOperation.StartOperation(Base, () =>
            {
                var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();

                graph.Clear();
                graph.Files.Current = graph.Files.Search<UploadFileWithIDSelector.fileID>(fileId);
                var graphExtension = graph.GetExtension<WikiFileMaintenanceESExt>();

                var revisions = graphExtension.RevisionsWithAction.Select();

                var pxResult = revisions.Select(joinResult => joinResult as PXResult<UploadFileRevisionNoData, ESignEnvelopeInfo>);
                foreach (ESignEnvelopeInfo envelope in pxResult)
                {
                    if (!string.IsNullOrEmpty(envelope?.LastStatus)
                    && envelope.LastStatus != EnvelopeStatus.New)
                    {
                        CheckStatus(graph, envelope, uploadGraph, taskGraph);
                    }
                }
            });
            return adapter.Get();
        }

        /// <summary>
        /// Create new smart panel with <see cref="ESignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> ESignSelected;
        [PXUIField(DisplayName = Messages.ESignSelected, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = Sprite.Main.DataEntry, CommitChanges = true)]
        public virtual IEnumerable eSignSelected(PXAdapter adapter)
        {
            if (RevisionsWithAction.Current != null)
            {
                var revision = RevisionsWithAction.Current;

                ESignEnvelopeInfo currentEnvelope = Envelope
                    .Search<ESignEnvelopeInfo.fileID, ESignEnvelopeInfo.fileRevisionID>
                    (revision.FileID, revision.FileRevisionID);

                Envelope.Current = currentEnvelope ?? CreateNewEnvelope();

                var result = Envelope.AskExt((graph, view) =>
                {
                    if (currentEnvelope != null && currentEnvelope.LastStatus != EnvelopeStatus.New)
                    {
                        var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                        var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                        CheckStatus(Base, Envelope.Current, uploadGraph, taskGraph);
                    }
                }, true);
                if (result == WebDialogResult.OK)
                {
                    PerformSendToESignRequest();
                }
            }
            return adapter.Get();
        }

        public PXAction<UploadFileWithIDSelector> SendToESign;
        [PXUIField(DisplayName = "Send", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = true, Enabled = true)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable sendToESign(PXAdapter adapter)
        {
            TryPerformAction(() => Base.Actions.PressSave(), exception => { });
            return adapter.Get();
        }

        /// <summary>
        /// Redirect to final uploaded signed version of <see cref="ESignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> ViewFile;
        [PXUIField(DisplayName = Messages.ViewFile, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewFile(PXAdapter adapter)
        {
            if (RevisionsWithAction.Current != null)
            {
                ESignEnvelopeInfo existingEnvelope = Envelope.Search<ESignEnvelopeInfo.fileID, ESignEnvelopeInfo.fileRevisionID>
                    (RevisionsWithAction.Current.FileID, RevisionsWithAction.Current.FileRevisionID);
                if (existingEnvelope?.CompletedFileID != null)
                {
                    var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                    UploadFileWithIDSelector file = graph.Files.Search<UploadFileWithIDSelector.fileID>(existingEnvelope.CompletedFileID);

                    if (file != null)
                    {
                        throw new PXRedirectToFileException(file.FileID, 1, true);
                    }
                }
            }
            return adapter.Get();
        }

        /// <summary>
        /// Save & Close smart panel related to current <see cref="ESignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> SaveAndCloseEnvelopeInfo;
        [PXUIField(DisplayName = Messages.ESignEnvelopeSaveAndClose, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable saveAndCloseEnvelopeInfo(PXAdapter adapter)
        {

            Base.Actions.PressSave();
            return adapter.Get();
        }

        /// <summary>
        /// Save smart panel related to current <see cref="ESignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> SaveEnvelopeInfo;

        [PXUIField(DisplayName = Messages.ESignEnvelopeSave, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable saveEnvelopeInfo(PXAdapter adapter)
        {
            Base.Actions.PressSave();
            return adapter.Get();
        }

        public PXDelete<UploadFileWithIDSelector> Delete;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXDeleteButton(ConfirmationMessage = null)]
        public virtual IEnumerable delete(PXAdapter adapter)
        {
            var revisions = RevisionsWithAction.Select();
            var envelopes =
                PXSelect
                    <ESignEnvelopeInfo,
                        Where<ESignEnvelopeInfo.fileID, Equal<Current<UploadFileWithIDSelector.fileID>>>>.Select(Base);
            if (envelopes.Any())
            {
                var result = Base.Files.View.Ask(Messages.ESignEnvelopeDeleteConfirm, MessageButtons.YesNo, true);
                if (result == WebDialogResult.Yes)
                {
                    var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                    var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                    var pxResult =
                        revisions.Select(
                            joinResult => joinResult as PXResult<UploadFileRevisionNoData, ESignEnvelopeInfo>);
                    foreach (ESignEnvelopeInfo envelope in pxResult)
                    {
                        if (envelope.LastStatus != EnvelopeStatus.New)
                        {
                            CheckStatus(Base, envelope, uploadGraph, taskGraph);
                        }

                        CleanBaseEnvelope(envelope);

                        if (envelope.IsActionsAvailable != null
                            && envelope.IsActionsAvailable.Value
                            && envelope.EnvelopeInfoID.HasValue)
                        {
                            VoidDeletedDocument(envelope);
                        }
                        else if (envelope.LastStatus == EnvelopeStatus.DocuSign.Created)
                        {
                            VoidDraftDocument(envelope);
                        }

                    }
                    Base.Delete.Press();
                }
            }
            else
            {
                var result = Base.Files.View.Ask(Messages.ESignEnvelopeDeleteFileConfirm, MessageButtons.OKCancel, true);
                if (result == WebDialogResult.OK)
                {
                    Base.Delete.Press();
                }
            }

            return adapter.Get();
        }

        public PXAction<UploadFileWithIDSelector> DeleteRow;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageKey = Sprite.Main.RecordDel)]
        public virtual IEnumerable deleteRow(PXAdapter adapter)
        {
            var revision = RevisionsWithAction.Current;

            if (revision != null)
            {
                ESignEnvelopeInfo envelope = PXSelect<ESignEnvelopeInfo,
                    Where<ESignEnvelopeInfo.fileID, Equal<Required<UploadFileRevisionNoData.fileID>>,
                        And
                            <ESignEnvelopeInfo.fileRevisionID,
                                Equal<Required<UploadFileRevisionNoData.fileRevisionID>>>>>.Select(Base, revision.FileID, revision.FileRevisionID);
                if (envelope != null)
                {
                    var result = Base.Files.View.Ask(Messages.ESignEnvelopeDeleteConfirm, MessageButtons.YesNo, true);
                    if (result == WebDialogResult.Yes)
                    {
                        if (envelope.LastStatus != EnvelopeStatus.New)
                        {
                            var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                            var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                            CheckStatus(Base, envelope, uploadGraph, taskGraph);
                        }

                        RevisionsWithAction.Delete(revision);
                        CleanBaseEnvelope(envelope);

                        if (envelope.IsActionsAvailable != null
                            && envelope.IsActionsAvailable.Value
                            && envelope.EnvelopeInfoID.HasValue)
                        {
                            VoidDeletedDocument(envelope);
                        }
                        else if (envelope.LastStatus == EnvelopeStatus.DocuSign.Created)
                        {
                            VoidDraftDocument(envelope);
                        }
                    }
                }
                else
                {
                    RevisionsWithAction.Delete(revision);
                }
            }
            return adapter.Get();
        }

        public ESignEnvelopeInfo CheckStatus(WikiFileMaintenance graph, ESignEnvelopeInfo envelope,
            UploadFileMaintenance uploadGraph, CRTaskMaint taskGraph)
        {
            graph.Clear();
            graph.Files.Current = graph.Files.Search<UploadFileWithIDSelector.fileID>(envelope.FileID);

            var graphExtension = graph.GetExtension<WikiFileMaintenanceESExt>();
            graphExtension.Envelope.Current = envelope;

            ESignAccount account = graphExtension.ESignAccount
                .Search<ESignAccount.accountID>(envelope.ESignAccountID);

            VerifyAccount(account);

            var model = GetBaseRequestModel(envelope, account);

            switch (account.ProviderType)
            {
                case ProviderTypes.DocuSign:
                    CheckDocuSignStatus(graph, envelope, uploadGraph, taskGraph, model, graphExtension);
                    break;
                case ProviderTypes.AdobeSign:
                    CheckAdobeSignStatus(graph, envelope, uploadGraph, taskGraph, account, model, graphExtension);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }

            return envelope;
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Create new <see cref="ESignEnvelopeInfo"/> envelope in acumatica DB.
        /// </summary>
        private ESignEnvelopeInfo CreateNewEnvelope()
        {
            ESignAccount account = ESignAccount.Select();
            VerifyAccount(account);

            var envelopeInfo = new ESignEnvelopeInfo
            {
                FileID = RevisionsWithAction.Current.FileID,
                FileRevisionID = RevisionsWithAction.Current.FileRevisionID,
                ESignAccountID = account.AccountID,
                LastStatus = EnvelopeStatus.New,
                Theme = " ",
                MessageBody = string.Empty,
                ProviderType = account.ProviderType
            };

            envelopeInfo = Envelope.Insert(envelopeInfo);
            CreateDefaultRecipients();

            return envelopeInfo;
        }

        /// <summary>
        /// Create default <see cref="ESignRecipient"/> recipients in acumatica DB.
        /// Search <see cref="Contact"/>contacts from privary view of the related note of current file.
        /// </summary>
        /// <param name="graphExtension"></param>
        private void CreateDefaultRecipients()
        {
            var noteSet = PXSelectJoin<NoteDoc, InnerJoin<Note, On<NoteDoc.noteID, Equal<Note.noteID>>>,
                Where<NoteDoc.fileID, Equal<Optional<UploadFileWithIDSelector.fileID>>>,
                OrderBy<Asc<NoteDoc.entityType>>>.Select(Base, RevisionsWithAction.Current.FileID);

            var listContactIds = new List<object>();
            var eSignRecipients = new List<ESignRecipient>();

            foreach (var pxResult in noteSet.Select(joinResult => joinResult as PXResult<NoteDoc, Note>))
            {
                if (pxResult == null)
                {
                    return;
                }

                var note = (Note)pxResult;
                if (note == null)
                {
                    return;
                }

                var type = PXBuildManager.GetType(note.EntityType, false);
                if (type != null && note.NoteID.HasValue)
                {
                    var entityHelper = new EntityHelper(Base);
                    var item = entityHelper.GetEntityRow(type, note.NoteID.Value);

                    var cache = Base.Caches[item.GetType()];

                    if (item.GetType() == typeof(Contact))
                    {
                        foreach (var field in cache.Fields)
                        {
                            var attributes = cache.GetAttributes(null, field).ToList();
                            var identityAttribute = attributes.OfType<PXDBIdentityAttribute>().FirstOrDefault();
                            if (identityAttribute != null)
                            {
                                var value = item.GetType().GetProperty(field).GetValue(item);
                                if (value is int)
                                {
                                    listContactIds.Add(value);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var field in cache.Fields)
                        {
                            var attributes = cache.GetAttributes(null, field).ToList();
                            var selectorAttribute = attributes.OfType<PXSelectorAttribute>().FirstOrDefault();
                            if (selectorAttribute != null)
                            {
                                if (selectorAttribute.Field.DeclaringType.GetTypeInfo()
                                                                              .ImplementedInterfaces.Contains(typeof(IContact)) ||
                                    selectorAttribute.Field.DeclaringType.GetTypeInfo()
                                                                              .ImplementedInterfaces.Contains(typeof(IContactBase)))
                                {
                                    var contactInfo = PXSelectorAttribute.Select(cache, item, selectorAttribute.FieldName);
                                    if (contactInfo == null) continue;
                                    string sEmail = string.Empty, sDisplayName = string.Empty;
                                    sEmail = contactInfo is IContactBase ?
                                                (contactInfo as IContactBase).EMail : contactInfo is IContact ?
                                                (contactInfo as IContact).Email : string.Empty;

                                    sDisplayName = contactInfo.GetType().GetProperty("DisplayName") != null ?
                                                        Convert.ToString(contactInfo.GetType().GetProperty("DisplayName").GetValue(contactInfo)) :
                                                        Convert.ToString(contactInfo.GetType().GetProperty("FullName").GetValue(contactInfo));
                                    if (string.IsNullOrEmpty(sDisplayName))
                                        sDisplayName = Convert.ToString(contactInfo.GetType().GetProperty("FullName").GetValue(contactInfo));
                                    if (!string.IsNullOrEmpty(sEmail) && !string.IsNullOrEmpty(sDisplayName))
                                    {
                                        if (!eSignRecipients.Any(x => x.Email == sEmail && x.Name == sDisplayName))
                                        {
                                            var recipient = new ESignRecipient
                                            {
                                                Email = sEmail,
                                                Name = sDisplayName
                                            };
                                            eSignRecipients.Add(recipient);
                                            Recipients.Insert(recipient);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            var list = new List<ESignRecipient>();
            listContactIds
                .Where(x => x != null)
                .Distinct()
                .ForEach(x => CreateDefaultRecipient(x, list));
        }

        /// <summary>
        /// Insert new <see cref="ESignRecipient"/> recipient in acumatica DB.
        /// </summary>
        private void CreateDefaultRecipient(object contactId, ICollection<ESignRecipient> rec)
        {
            Contact contact = Contacts.Search<Contact.contactID>(contactId).FirstOrDefault();
            if (!string.IsNullOrEmpty(contact?.EMail)
                && !string.IsNullOrEmpty(contact.FullName)
                && !rec.Any(x => x.Email == contact.EMail && x.Name == contact.FullName))
            {
                var recipient = new ESignRecipient
                {
                    Email = contact.EMail,
                    Name = contact.FullName
                };

                rec.Add(recipient);
                Recipients.Insert(recipient);
            }
        }

        private CreateEnvelopeRequestModel CreateSendRequestModel(ESignAccount account, WikiFileMaintenanceESExt graphEsExt)
        {
            if (graphEsExt.Envelope.Current.FileID != null && graphEsExt.Envelope.Current.FileRevisionID != null)
            {
                var fileGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                fileGraph.Clear();
                fileGraph.Files.Current = fileGraph.Files.Search<UploadFileWithIDSelector.fileID>(graphEsExt.Envelope.Current.FileID);

                var file = fileGraph.GetFile(Envelope.Current.FileID.Value, Envelope.Current.FileRevisionID.Value);

                var fileExtension = Path.GetExtension(file.Name);
                if (!Constants.AcumaticaESignFileExtensions.Contains(fileExtension))
                {
                    throw new AcumaticaESignIncorrectFileTypeException();
                }

                var requestModel = new CreateEnvelopeRequestModel
                {
                    EnvelopeInfo = graphEsExt.Envelope.Current,
                    Recipients = new List<ESignRecipient>(),
                    CarbonRecipients = new List<ESignRecipient>(),
                    FileInfo = file,
                    ESignAccount = account,
                    EnvelopeId = graphEsExt.Envelope.Current.EnvelopeID
                };

                InitRecipientsForSendRequest(requestModel, graphEsExt);

                return requestModel;
            }

            throw new PXException(Messages.ESignEnvelopeNotExists);
        }

        private void InitRecipientsForSendRequest(CreateEnvelopeRequestModel requestModel, WikiFileMaintenanceESExt graphEsExt)
        {
            var recipients = graphEsExt.Recipients.Select();

            foreach (ESignRecipient recipient in recipients)
            {
                switch (recipient.Type)
                {
                    case ESignRecipient.RecipientTypes.Signer:
                        requestModel.Recipients.Add(recipient);
                        break;
                    case ESignRecipient.RecipientTypes.CopyRecipient:
                        requestModel.CarbonRecipients.Add(recipient);
                        break;
                }
            }
        }

        private void SendCreateEnvelopeRequest(DocuSignService dsService, ESignEnvelopeInfo envelope,
            ESignAccount account, WikiFileMaintenanceESExt graphEsExt)
        {
            switch (account.ProviderType)
            {
                case DocuSignProvider:
                    SendDocuSignEnvelopeForCreate(dsService, envelope, account, graphEsExt);
                    break;
                case AdobeSignProvider:
                    SendAdobeSignEnvelope(envelope, account, graphEsExt);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }
        }

        private static void UpdateSentAdobeSignEnvelope(ESignEnvelopeInfo envelope, AgreementCreationEntity sendEnvelope)
        {
            envelope.EnvelopeID = sendEnvelope.agreementId;
            envelope.SendDate = DateTime.Now;
            envelope.LastStatus = EnvelopeStatus.AdobeSign.Authoring;
            envelope.ReviewUrl = sendEnvelope.url;
        }

        private static AgreementCreationInfoModel GetAgreementCreationInfoModel(ESignEnvelopeInfo envelope,
            CreateEnvelopeRequestModel sendRequest, TransientDocumentEntity sendResponse)
        {
            var documentCreationInfoModel = CreateDocumentCreationInfoModel(envelope, sendRequest, sendResponse);
            var model = new AgreementCreationInfoModel
            {
                documentCreationInfo = documentCreationInfoModel,
                options = new InteractiveOptionsModel()
            };
            return model;
        }

        private static DocumentCreationInfoModel CreateDocumentCreationInfoModel(ESignEnvelopeInfo envelope,
            CreateEnvelopeRequestModel sendRequest, TransientDocumentEntity sendResponse)
        {
            var recipient = sendRequest.Recipients.OrderByDescending(x => x.Position).ToList();
            var list = new List<RecipientSetInfosModel>();
            for (var i = 0; i < recipient.Count; i++)
            {
                var eSignRecipient = recipient[i];
                if (eSignRecipient.Position != null)
                {
                    eSignRecipient.Position = i + 1;
                }
                list.Add(CreateRecipientSetInfosModel(eSignRecipient));
            }
            var recipientSetInfos = list.ToArray();
            var fileInfosModels = new[]
            {
                new FileInfosModel
                {
                    transientDocumentId = sendResponse.transientDocumentId
                }
            };
            return new DocumentCreationInfoModel
            {
                message = envelope.MessageBody,
                name = envelope.Theme,
                daysUntilSigningDeadline = envelope.ExpiredDays.Value,
                reminderFrequency = envelope.SendReminders != true ? string.Empty : envelope.ReminderType,
                signatureType = "ESIGN",
                ccs = sendRequest.CarbonRecipients.Select(x => x.Email).ToArray(),
                recipientSetInfos = recipientSetInfos,
                fileInfos = fileInfosModels
            };
        }

        private static RecipientSetInfosModel CreateRecipientSetInfosModel(ESignRecipient x)
        {
            return new RecipientSetInfosModel
            {
                recipientSetMemberInfos = new[] { new RecipientSetMemberInfosModel { email = x.Email } },
                recipientSetRole = "SIGNER",
                signingOrder = x.Position ?? 1
            };
        }

        private void SendUpdateEnvelopeRequest(DocuSignService dsService, ESignEnvelopeInfo envelope,
            ESignAccount account, WikiFileMaintenanceESExt graphEsExt)
        {
            switch (account.ProviderType)
            {
                case DocuSignProvider:
                    SendDocuSignEnvelopeForUpdate(dsService, envelope, account, graphEsExt);
                    break;
                case AdobeSignProvider:
                    throw new PXRedirectToUrlException(envelope.ReviewUrl, string.Empty);
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }
        }

        private void SendDocuSignEnvelopeForUpdate(DocuSignService dsService, ESignEnvelopeInfo envelope, ESignAccount account,
            WikiFileMaintenanceESExt graphEsExt)
        {
            var updateRequest = CreateSendRequestModel(account, graphEsExt);
            updateRequest.EnvelopeInfo.EnvelopeID = envelope.EnvelopeID;

            var updateResponse = dsService.UpdateEnvelope(updateRequest);

            graphEsExt.Envelope.Update(envelope);
            graphEsExt.Base.Actions.PressSave();

            throw new PXRedirectToUrlException(updateResponse.ViewUrl.Url, string.Empty);
        }

        private ESignEnvelopeInfo GetActualEnvelope(WikiFileMaintenance graph, ESignEnvelopeInfo existingEnvelope)
        {
            var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
            var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
            return existingEnvelope.LastStatus != EnvelopeStatus.New
                ? CheckStatus(graph, existingEnvelope, uploadGraph, taskGraph)
                : existingEnvelope;
        }

        private static void UpdateEnvelope(ESignEnvelopeInfo envelope,
            GetEnvelopeHistoryResponseModel history, WikiFileMaintenanceESExt graph)
        {
            envelope.ActivityDate = DateTime.Parse(history.Envelope.LastModifiedDateTime);
            envelope.LastStatus = history.Envelope.Status;
            envelope.MessageBody = history.Envelope.EmailBlurb;
            envelope.Theme = history.Envelope.EmailSubject;
            envelope.ExpiredDays = int.Parse(history.Notification.Expirations.ExpireAfter);
            envelope.WarnDays = int.Parse(history.Notification.Expirations.ExpireWarn);
            envelope.FirstReminderDay = int.Parse(history.Notification.Reminders.ReminderDelay);
            envelope.ReminderFrequency = int.Parse(history.Notification.Reminders.ReminderFrequency);
            envelope.SendReminders = bool.Parse(history.Notification.Reminders.ReminderEnabled);

            DateTime expirationDate;
            envelope.ExpirationDate = DateTime.TryParse(history.Envelope.SentDateTime, out expirationDate)
                ? expirationDate.AddDays(envelope.ExpiredDays.Value)
                : envelope.SendDate?.AddDays(envelope.ExpiredDays.Value);

            graph.Envelope.Update(envelope);
        }

        private void UpdateCopyRecipients(IEnumerable<CarbonCopy> carbonCopies, WikiFileMaintenanceESExt graphEsExt)
        {
            foreach (var carbonCopy in carbonCopies)
            {
                var recipient = new ESignRecipient
                {
                    Email = carbonCopy.Email,
                    Name = carbonCopy.Name,
                    Status = carbonCopy.Status,
                    DeliveredDateTime = carbonCopy.DeliveredDateTime != null
                        ? Convert.ToDateTime(carbonCopy.DeliveredDateTime)
                        : default(DateTime?),
                    SignedDateTime = carbonCopy.SignedDateTime != null
                        ? Convert.ToDateTime(carbonCopy.SignedDateTime)
                        : default(DateTime?),
                    Type = ESignRecipient.RecipientTypes.CopyRecipient,
                    CustomMessage = carbonCopy.Note,
                    Position = string.IsNullOrEmpty(carbonCopy.RoutingOrder)
                    ? (int?)null
                    : int.Parse(carbonCopy.RoutingOrder)
                };
                graphEsExt.Recipients.Insert(recipient);

            }
        }

        private static void UpdateDocuSignSigners(IEnumerable<Signer> signers, List<EnvelopeAuditEvent> events, WikiFileMaintenanceESExt graph)
        {
            foreach (var signer in signers)
            {
                var recipient = new ESignRecipient
                {
                    Email = signer.Email,
                    Name = signer.Name,
                    Status = signer.Status,
                    DeliveredDateTime = signer.DeliveredDateTime != null
                        ? Convert.ToDateTime(signer.DeliveredDateTime)
                        : default(DateTime?),
                    SignedDateTime = signer.SignedDateTime != null
                        ? Convert.ToDateTime(signer.SignedDateTime)
                        : default(DateTime?),
                    Type = ESignRecipient.RecipientTypes.Signer,
                    CustomMessage = signer.Note,
                    Position = string.IsNullOrEmpty(signer.RoutingOrder)
                        ? (int?)null
                        : int.Parse(signer.RoutingOrder),
                    IPAddress = GetDocuSignRecipientIpAddress(events, signer.UserId)

                };
                graph.Recipients.Insert(recipient);

            }
        }

        private static string GetDocuSignRecipientIpAddress(List<EnvelopeAuditEvent> events, string userId)
        {
            var ipAddress = string.Empty;
            foreach (var envelopeEvent in events)
            {
                if (envelopeEvent.EventFields.Find(x => x.Name == "UserId").Value == userId
                    && ((envelopeEvent.EventFields.Find(x => x.Name == "Action").Value == "Signed")
                    || (envelopeEvent.EventFields.Find(x => x.Name == "Action").Value == "Voided")
                    || (envelopeEvent.EventFields.Find(x => x.Name == "Action").Value == "Viewed")))
                {
                    ipAddress = envelopeEvent.EventFields.Find(x => x.Name == "ClientIPAddress").Value;
                }
            }
            return ipAddress;
        }

        private static void UpdateAdobeSignSigners(ESignEnvelopeInfo envelope, AgreementInfoEntity info, WikiFileMaintenanceESExt graphEsExt, AdobeSignClient client)
        {
            foreach (var signer in info.participantSetInfos)
            {
                if (signer.roles != "[\"SENDER\"]")
                {
                    var recipient = new ESignRecipient
                    {
                        EnvelopeInfoID = envelope.EnvelopeInfoID,
                        Email = signer.participantSetMemberInfos[0].email,
                        Name = signer.status == "SIGNED"
                            ? GetSignerName(envelope.EnvelopeID, signer.participantSetMemberInfos[0].email, client)
                            : null,
                        Status = signer.status.ToLower(),
                        DeliveredDateTime = GetDeliveredDateTime(info.events, signer.participantSetMemberInfos[0].email)?.ToLocalTime(),
                        SignedDateTime = GetSignedDateTime(info.events, signer.participantSetMemberInfos[0].email)?.ToLocalTime(),
                        Type = signer.roles.Contains("\"SIGNER\"")
                            ? ESignRecipient.RecipientTypes.Signer
                            : ESignRecipient.RecipientTypes.CopyRecipient,
                        CustomMessage = signer.privateMessage,
                        Position = signer.signingOrder,
                        IPAddress = GetAdobeSignRecipientIpAddress(info.events, signer.participantSetMemberInfos[0].email)
                    };

                    graphEsExt.Recipients.Insert(recipient);
                }

            }
        }

        private static string GetSignerName(string envelopeId, string email, AdobeSignClient client)
        {
            var agreementFormData = ESignApiExecutor.TryExecute(() => client.DocumentsService.GetAgreementFormData(envelopeId), client);
            var str = System.Text.Encoding.Default.GetString(agreementFormData);
            using (TextReader sr = new System.IO.StringReader(str))
            {
                var csv = new CsvReader(sr);
                var records = csv.GetRecords<CsvEntity>().ToList();
                foreach (var record in records)
                {
                    if (record.email == email)
                    {
                        return string.Concat(record.first, " ", record.last);
                    }
                }
            }
            return null;
        }

        private static string GetAdobeSignRecipientIpAddress(IEnumerable<DocumentHistoryEvent> events, string email)
        {
            var ipAddress = string.Empty;
            foreach (var envelopeEvent in events)
            {
                if (envelopeEvent.participantEmail == email)
                {
                    ipAddress = envelopeEvent.actingUserIpAddress;
                }
            }
            return ipAddress;
        }

        private static DateTime? GetDeliveredDateTime(IEnumerable<DocumentHistoryEvent> events, string email)
        {
            foreach (var envelopeEvent in events)
            {
                if (envelopeEvent.participantEmail == email && envelopeEvent.type == "EMAIL_VIEWED")
                {
                    return envelopeEvent.date;
                }
            }
            return null;
        }

        private static DateTime? GetSignedDateTime(IEnumerable<DocumentHistoryEvent> events, string email)
        {
            foreach (var envelopeEvent in events)
            {
                if (envelopeEvent.participantEmail == email && envelopeEvent.type == "ESIGNED")
                {
                    return envelopeEvent.date;
                }
            }
            return null;
        }

        /// <summary>
        /// Used for creating new task <see cref="CRActivity"/> after current envelope signed.
        /// </summary>
        private void CreateTaskForRelatedOwner(ESignEnvelopeInfo envelope, FileInfo fileInfo,
            DateTime completedDateTime, CRTaskMaint graph)
        {
            graph.Clear();

            var activity = graph.Tasks.Insert();
            activity.PercentCompletion = 0;
            activity.OwnerID = envelope.CreatedByID;
            activity.Subject = $"Document {fileInfo.FullName} was signed.";
            activity.ClassID = CRActivityClass.Task;
            activity.StartDate = completedDateTime;
            graph.Actions.PressSave();
        }

        /// <summary>
        /// Used for creating new file <see cref="UploadFile"/> after current envelope completed.
        /// </summary>
        private FileInfo CreateCompletedFile(ESignEnvelopeInfo envelope, WikiFileMaintenance graph,
            WikiFileMaintenanceESExt graphEsExt, UploadFileMaintenance uploadGraph, byte[] document)
        {
            uploadGraph.Clear();

            graph.Files.Current = PXSelect<UploadFileWithIDSelector,
                Where<UploadFileWithIDSelector.fileID, Equal<Required<UploadFileWithIDSelector.fileID>>>>
                .Select(Base, envelope.FileID, envelope.FileRevisionID);

            var currentFile = uploadGraph.GetFile(envelope.FileID.Value);

            var fileExtension = GetFileExtension(currentFile.OriginalName);
            var signedFileName = currentFile.FullName.Replace(fileExtension, " ESigned.pdf");
            var uniqueFileName = CreateFileWithUniqueName(signedFileName, envelope.FileRevisionID);
            var signedFile = new FileInfo(Guid.NewGuid(), uniqueFileName, null, document);
            uploadGraph.SaveFile(signedFile);

            NoteDoc currentNoteDoc =
                PXSelect<NoteDoc, Where<NoteDoc.fileID, Equal<Required<NoteDoc.fileID>>>>.Select(Base,
                    envelope.FileID);

            if (currentNoteDoc != null)
            {
                var noteDoc = new NoteDoc
                {
                    NoteID = currentNoteDoc.NoteID,
                    FileID = signedFile.UID
                };

                graph.EntitiesRecords.Insert(noteDoc);
            }
            var envelopeInfo = new ESignEnvelopeInfo
            {
                FileID = signedFile.UID,
                FileRevisionID = 1,
                ESignAccountID = envelope.ESignAccountID,
                LastStatus = envelope.ProviderType == DocuSignProvider
                    ? EnvelopeStatus.DocuSign.Completed
                    : EnvelopeStatus.AdobeSign.Signed,
                ActivityDate = envelope.LastModifiedDateTime,
                Theme = envelope.Theme,
                MessageBody = string.Empty,
                IsFinalVersion = true,
                EnvelopeID = envelope.EnvelopeID
            };

            envelope.CompletedFileID = signedFile.UID;
            envelope.CompletedFileName = signedFile.OriginalName;
            graphEsExt.Envelope.Update(envelope);

            graphEsExt.Envelope.Insert(envelopeInfo);

            return currentFile;
        }

        private string CreateFileWithUniqueName(string fileName, int? fileRevisionId)
        {
            var fileExtension = GetFileExtension(fileName);
            var fileBase = fileName.Replace(fileExtension, string.Empty);
            return $"{fileBase} ({fileRevisionId}){fileExtension}";

        }

        private string GetFileExtension(string fileName)
        {
            var fileExtension = fileName
                .Substring(fileName
                    .IndexOf(".", StringComparison.Ordinal));
            return fileExtension;
        }

        private void VerifyStatus(ESignEnvelopeInfo existingEnvelope)
        {
            if (existingEnvelope.LastStatus != EnvelopeStatus.New &&
                existingEnvelope.LastStatus != EnvelopeStatus.DocuSign.Created &&
                existingEnvelope.LastStatus != EnvelopeStatus.AdobeSign.Authoring)
            {
                throw new PXSetPropertyException(Messages.EnvelopeStatusChanged,
                    PXErrorLevel.Error, typeof(ESignEnvelopeInfo.eSignAccountID).Name);
            }
        }

        private void CleanCache()
        {
            Envelope.Cache.Clear();
            Envelope.Cache.ClearQueryCache();
            Recipients.Cache.Clear();
            Recipients.Cache.ClearQueryCache();
        }

        private BaseRequestModel GetBaseRequestModel(ESignEnvelopeInfo envelope, ESignAccount dsAccount)
        {
            return new BaseRequestModel
            {
                EnvelopeId = envelope.EnvelopeID,
                ESignAccount = dsAccount
            };
        }

        private bool IsStatusChangeToComplete(ESignEnvelopeInfo envelope, string currentStatus)
        {
            return currentStatus == EnvelopeStatus.DocuSign.Completed
                   && envelope.LastStatus != EnvelopeStatus.DocuSign.Completed
                   || currentStatus == EnvelopeStatus.AdobeSign.Signed
                   && envelope.LastStatus != EnvelopeStatus.AdobeSign.Signed;
        }

        private void VerifyRecipients()
        {
            var recipients = Recipients.Select();
            if (recipients.Count == 0)
            {
                throw new PXException(Messages.ESignRecipientsRequired);
            }
            var groupBy = recipients.FirstTableItems.GroupBy(x => new { x.Position, x.Email, x.Name });
            if (groupBy.Any(x => x.Count() > 1))
            {
                throw new PXException(Messages.ESignRecipientsUnique);
            }
        }

        private static void VerifyAccount(ESignAccount account)
        {
            if (account == null || account.IsActive != true)
            {
                throw new PXException(Messages.ESignAccountNotExists);
            }
        }

        private void PerformSendToESignRequest()
        {
            Base.Actions.PressSave();
            VerifyRecipients();

            var envelopeInfoId = Envelope.Current.EnvelopeInfoID;

            PXLongOperation.StartOperation(Base, () =>
            {
                var dsService = new DocuSignService();
                var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                var envelope = GetEnvelopeInfo(graph, envelopeInfoId);

                VerifyStatus(envelope);

                var actualEnvelope = GetActualEnvelope(graph, envelope);

                var graphExtension = graph.GetExtension<WikiFileMaintenanceESExt>();
                graphExtension.Envelope.Current = actualEnvelope;

                var dsAccount = graphExtension.ESignAccount
                    .Search<ESignAccount.accountID>(envelope.ESignAccountID);
                var model = GetBaseRequestModel(actualEnvelope, dsAccount);

                VerifyAccount(dsAccount);
                VerifyStatus(actualEnvelope);

                if (actualEnvelope.LastStatus != EnvelopeStatus.DocuSign.Created
                || !dsService.IsFileExist(model)
                || actualEnvelope.LastStatus != EnvelopeStatus.AdobeSign.Authoring)
                {
                    SendCreateEnvelopeRequest(dsService, actualEnvelope, dsAccount, graphExtension);
                }
                else
                {
                    SendUpdateEnvelopeRequest(dsService, actualEnvelope, dsAccount, graphExtension);
                }
            });
        }

        private void VoidDeletedDocument(ESignEnvelopeInfo envelope)
        {
            ESignAccount account = ESignAccount.Search<ESignAccount.accountID>(envelope.ESignAccountID);
            VerifyAccount(account);

            switch (account.ProviderType)
            {
                case DocuSignProvider:
                    ESignDocumentSummaryEnq.VoidDocuSignEnvelope(account, envelope, Messages.DefaultEnvelopeVoidReason);
                    break;
                case AdobeSignProvider:
                    ESignDocumentSummaryEnq.CancelAdobeSignEnvelope(account, envelope, Messages.DefaultEnvelopeVoidReason);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }

            Envelope.Cache.SetStatus(envelope, PXEntryStatus.Deleted);
        }

        private void VoidDraftDocument(ESignEnvelopeInfo envelope)
        {
            ESignAccount account = ESignAccount.Search<ESignAccount.accountID>(envelope.ESignAccountID);
            VerifyAccount(account);

            switch (account.ProviderType)
            {
                case DocuSignProvider:
                    ESignDocumentSummaryEnq.VoidDraftDocuSignEnvelope(account, envelope);
                    break;
                case AdobeSignProvider:
                    ESignDocumentSummaryEnq.VoidAdobeSignEnvelope(envelope, account);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }

            Envelope.Cache.SetStatus(envelope, PXEntryStatus.Deleted);
        }

        private void CleanExisitingRecipients(ESignEnvelopeInfo envelope, WikiFileMaintenanceESExt graphExtension)
        {
            var recipients = PXSelect<ESignRecipient,
                Where<ESignRecipient.envelopeInfoID,
                    Equal<Required<ESignRecipient.envelopeInfoID>>>>.Select(Base, envelope.EnvelopeInfoID);

            foreach (var pxResult in recipients)
            {
                graphExtension.Recipients.Delete(pxResult);
            }
        }

        private void CleanBaseEnvelope(ESignEnvelopeInfo envelope)
        {
            if (envelope.IsFinalVersion.HasValue && envelope.IsFinalVersion.Value)
            {
                ESignEnvelopeInfo baseEnvelope = PXSelect<ESignEnvelopeInfo,
                    Where<ESignEnvelopeInfo.completedFileID,
                        Equal<Required<ESignEnvelopeInfo.completedFileID>>>>
                    .Select(Base, envelope.FileID);

                baseEnvelope.CompletedFileName = null;
                baseEnvelope.CompletedFileID = null;

                Envelope.Update(baseEnvelope);
            }
        }

        private void UpdateRecipientsGridLayout(string providerType)
        {
            var areAdditionalFieldsAvailable = providerType == ProviderTypes.DocuSign;
            var persistingCheck = areAdditionalFieldsAvailable
                ? PXPersistingCheck.Null
                : PXPersistingCheck.Nothing;
            PXUIFieldAttribute.SetVisible<ESignRecipient.name>(Recipients.Cache, null, areAdditionalFieldsAvailable);
            PXUIFieldAttribute.SetVisible<ESignRecipient.customMessage>(Recipients.Cache, null, areAdditionalFieldsAvailable);
            PXDefaultAttribute.SetPersistingCheck<ESignRecipient.name>(Recipients.Cache, null, persistingCheck);
        }

        private void SendAdobeSignEnvelope(ESignEnvelopeInfo envelope, ESignAccount account, WikiFileMaintenanceESExt graphEsExt)
        {
            var sendRequest = CreateSendRequestModel(account, graphEsExt);
            var client = AdobeSignClientBuilder.Build(account);
            var sendResponse = ESignApiExecutor.TryExecute(() => client.DocumentsService.SendDocument(
                sendRequest.FileInfo.BinData, sendRequest.FileInfo.Name), client);
            var model = GetAgreementCreationInfoModel(envelope, sendRequest, sendResponse);
            var sendEnvelope =
                ESignApiExecutor.TryExecute(() => client.DocumentsService.CreateAgreement(model), client);

            UpdateSentAdobeSignEnvelope(envelope, sendEnvelope);
            graphEsExt.Envelope.Update(envelope);
            graphEsExt.Base.Actions.PressSave();
            throw new PXRedirectToUrlException(sendEnvelope.url, string.Empty);
        }

        private void SendDocuSignEnvelopeForCreate(DocuSignService dsService, ESignEnvelopeInfo envelope, ESignAccount account,
            WikiFileMaintenanceESExt graphEsExt)
        {
            var sendRequest = CreateSendRequestModel(account, graphEsExt);
            var sendResponse = dsService.CreateEnvelope(sendRequest);

            envelope.EnvelopeID = sendResponse.EnvelopeSummary.EnvelopeId;
            envelope.ActivityDate = DateTime.Parse(sendResponse.EnvelopeSummary.StatusDateTime);
            envelope.SendDate = DateTime.Parse(sendResponse.EnvelopeSummary.StatusDateTime);
            envelope.LastStatus = sendResponse.EnvelopeSummary.Status;

            graphEsExt.Envelope.Update(envelope);
            graphEsExt.Base.Actions.PressSave();

            throw new PXRedirectToUrlException(sendResponse.ViewUrl.Url, string.Empty);
        }

        private static void UpdateEnvelope(ESignEnvelopeInfo envelope, AgreementInfoEntity history,
            WikiFileMaintenanceESExt graph)
        {
            envelope.ActivityDate = history.events.OrderByDescending(x => x.date).First().date.ToLocalTime();
            envelope.LastStatus = history.status;
            envelope.MessageBody = history.message;
            envelope.Theme = history.name;
            envelope.ExpirationDate = history.expiration;

            graph.Envelope.Update(envelope);
        }

        private void CheckAdobeSignStatus(WikiFileMaintenance graph, ESignEnvelopeInfo envelope,
            UploadFileMaintenance uploadGraph, CRTaskMaint taskGraph, ESignAccount dsAccount, BaseRequestModel model,
            WikiFileMaintenanceESExt graphExtension)
        {
            var client = AdobeSignClientBuilder.Build(dsAccount);
            var history = ESignApiExecutor.TryExecute(() => client.DocumentsService.GetAgreementStatus(model.EnvelopeId), client);

            using (var ts = new PXTransactionScope())
            {
                if (IsStatusChangeToComplete(envelope, history.status))
                {
                    var document = client.DocumentsService.GetAgreement(envelope.EnvelopeID);

                    var fileInfo = CreateCompletedFile(envelope,
                        graph, graphExtension, uploadGraph, document);

                    CreateTaskForRelatedOwner(envelope, fileInfo, DateTime.Now, taskGraph);
                }

                CleanExisitingRecipients(envelope, graphExtension);
                UpdateAdobeSignSigners(envelope, history, graphExtension, client);
                UpdateEnvelope(envelope, history, graphExtension);

                graph.Actions.PressSave();
                ts.Complete();
            }
        }

        private void CheckDocuSignStatus(WikiFileMaintenance graph, ESignEnvelopeInfo envelope,
            UploadFileMaintenance uploadGraph, CRTaskMaint taskGraph, BaseRequestModel model,
            WikiFileMaintenanceESExt graphExtension)
        {
            var docusignService = new DocuSignService();
            var history = docusignService.GetEnvelopeHistory(model);

            using (var ts = new PXTransactionScope())
            {
                if (IsStatusChangeToComplete(envelope, history.Envelope.Status))
                {
                    var responseModel = docusignService.GetEnvelopeDocument(model);
                    var fileInfo = CreateCompletedFile(envelope, graph, graphExtension, uploadGraph, responseModel.Document);

                    var completedDate = DateTime.Parse(history.Envelope.CompletedDateTime);
                    CreateTaskForRelatedOwner(envelope, fileInfo, completedDate, taskGraph);
                }

                UpdateEnvelope(envelope, history, graphExtension);

                CleanExisitingRecipients(envelope, graphExtension);
                UpdateDocuSignSigners(history.Recipients.Signers, history.Events.AuditEvents, graphExtension);
                UpdateCopyRecipients(history.Recipients.CarbonCopies, graphExtension);

                graph.Actions.PressSave();
                ts.Complete();
            }
        }

        private static void TryPerformAction(Action action, Action<Exception> negativeAction)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                negativeAction(exception);
            }
        }

        private static ESignEnvelopeInfo GetEnvelopeInfo(WikiFileMaintenance maintenanceGraph, int? envelopeInfoId)
        {
            return new PXSelect<ESignEnvelopeInfo,
                Where<ESignEnvelopeInfo.envelopeInfoID,
                    Equal<Required<ESignEnvelopeInfo.envelopeInfoID>>>>(maintenanceGraph)
                .SelectSingle(envelopeInfoId);
        }

        #endregion
    }
}