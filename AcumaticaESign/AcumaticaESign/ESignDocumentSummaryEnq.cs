using System.Collections;
using AcumaticaESign.SM;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.SM;

namespace AcumaticaESign
{
    /// <summary>
    /// Represent inqury logic fo the <see cref="ESignEnvelopeInfo"/> envelopes
    /// Records of this type can be viewed through the ESign Central (ES.40.10.00) screen.
    /// </summary>
    [DashboardType((int)DashboardTypeAttribute.Type.Default)]
    public class ESignDocumentSummaryEnq : PXGraph<ESignDocumentSummaryEnq>
    {
        private const string DocuSignProvider = ProviderTypes.DocuSign;
        private const string AdobeSignProvider = ProviderTypes.AdobeSign;
        
        #region Selects

        public PXFilter<EnvelopeFilter> Filter;

        [PXFilterable]
        public PXSelectJoinGroupBy<ESignEnvelopeInfo,
                InnerJoin<UploadFileRevision,
                    On<UploadFileRevision.fileID, Equal<ESignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<ESignEnvelopeInfo.fileRevisionID>>>,
                InnerJoin<ESignAccount, On<ESignEnvelopeInfo.eSignAccountID, Equal<ESignAccount.accountID>>,
                InnerJoin<UploadFileWithIDSelector, On<UploadFileWithIDSelector.fileID, Equal<UploadFileRevision.fileID>>,
                InnerJoin<SiteMap, On<SiteMap.screenID, Equal<UploadFileWithIDSelector.primaryScreenID>>>>>>,
                Where<ESignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                    And<ESignEnvelopeInfo.isFinalVersion, NotEqual<True>>>,
                Aggregate<GroupBy<ESignEnvelopeInfo.envelopeInfoID>>> Envelopes;

        public PXFilter<VoidRequestFilter> VoidRequest;

        public PXSelect<ESignAccount,
            Where<ESignAccount.accountID, Equal<Required<ESignAccount.accountID>>>> ESignAccount;

        public PXSelect<SiteMap> SiteMap;

        public PXCancel<ESignEnvelopeInfo> Cancel;

        public virtual IEnumerable envelopes()
        {
            PXSelectBase<ESignEnvelopeInfo> query = new PXSelectJoinGroupBy<ESignEnvelopeInfo,
                InnerJoin<UploadFileRevision,
                    On<UploadFileRevision.fileID, Equal<ESignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<ESignEnvelopeInfo.fileRevisionID>>>,
                InnerJoin<ESignAccount, On<ESignEnvelopeInfo.eSignAccountID, Equal<ESignAccount.accountID>>,
                InnerJoin<UploadFileWithIDSelector, On<UploadFileWithIDSelector.fileID, Equal<UploadFileRevision.fileID>>,
                InnerJoin<SiteMap, On<SiteMap.screenID, Equal<UploadFileWithIDSelector.primaryScreenID>>>>>>,
                Where<ESignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                    And<ESignEnvelopeInfo.isFinalVersion, NotEqual<True>>>,
                Aggregate<GroupBy<ESignEnvelopeInfo.envelopeInfoID>>>(this);

            EnvelopeFilter filter = Filter.Current;

            if (filter.OwnerID != null)
            {
                query.WhereAnd<Where<ESignEnvelopeInfo.createdByID, Equal<Current<EnvelopeFilter.ownerID>>>>();
            }

            return query.Select();
        }

        #endregion

        #region Events
        
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName", Messages.SiteMapTitleDisplayName)]
        protected void SiteMap_Title_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName", Messages.ESignEnvelopeInfo.CreatedDate)]
        protected void ESignEnvelopeInfo_CreatedDateTime_CacheAttached(PXCache sender)
        {
        }

        protected virtual void _(Events.RowSelected<EnvelopeFilter> args)
        {
            var filter = args.Row;

            if (filter?.MyOwner != null)
            {
                PXUIFieldAttribute.SetEnabled<EnvelopeFilter.ownerID>(Filter.Cache, null, !filter.MyOwner.Value);
            }
        }

        public ESignDocumentSummaryEnq()
        {
            if (Envelopes.Any())
            {
                ViewESign.SetEnabled(true);
                CheckStatus.SetEnabled(true);
            }
            else
            {
                ViewESign.SetEnabled(false);
                CheckStatus.SetEnabled(false);
            }
        }

        #endregion

        #region Actions

        public PXAction<ESignEnvelopeInfo> Delete;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.Remove)]
        public virtual IEnumerable delete(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelopeInfoId = Envelopes.Current.EnvelopeInfoID;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceESExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();

                    var envelope = GetEnvelopeInfo(maintenanceGraph, envelopeInfoId);
                    var account = fileMaintenanceExtension.ESignAccount.SelectSingle(envelope.ESignAccountID);

                    fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);

                    VerifyAccount(account);

                    switch (account.ProviderType)
                    {
                        case DocuSignProvider:
                            VoidDocuSignEnvelope(envelope, account);
                            break;
                        case AdobeSignProvider:
                            VoidAdobeSignEnvelope(envelope, account);
                            break;
                        default:
                            throw new PXException(Messages.ProviderTypeIsMissing);
                    }
                    var graphExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceESExt>();
                    graphExtension.Envelope.Delete(envelope);
                    maintenanceGraph.Actions.PressSave();
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Redirect to <see cref="WikiFileMaintenance"/> screen with current file.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> ViewFile;
        [PXUIField(DisplayName = Messages.ViewFile, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewFile(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                WikiFileMaintenance graph = CreateInstance<WikiFileMaintenance>();
                graph.Files.Current = graph.Files.Search<UploadFileWithIDSelector.fileID>(Envelopes.Current.FileID);

                throw new PXRedirectRequiredException(graph, string.Empty) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            return adapter.Get();
        }

        /// <summary>
        /// Redirect to <see cref="ESignEnvelopeEnq"/> screen with current file.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> ViewHistory;
        [PXUIField(DisplayName = Messages.ViewHistory, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewHistory(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                ESignEnvelopeEnq graph = CreateInstance<ESignEnvelopeEnq>();
                graph.Envelopes.Current = graph.Envelopes.Search<ESignEnvelopeInfo.envelopeInfoID>(Envelopes.Current.EnvelopeInfoID);

                throw new PXRedirectRequiredException(graph, string.Empty) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            return adapter.Get();
        }

        /// <summary>
        /// Open smart panel with <see cref="VoidRequest"/> void reason.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> VoidEnvelope;
        [PXButton]
        [PXUIField(DisplayName = Messages.Void, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable voidEnvelope(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var voidReason = VoidRequest.Current.VoidReason;

                VoidRequest.Cache.Clear();
                VoidRequest.Cache.ClearQueryCache();

                if (VoidRequest.AskExt(true) == WebDialogResult.OK)
                {
                    if (Envelopes.Current != null)
                    {
                        var envelopeInfoId = Envelopes.Current.EnvelopeInfoID;

                        PXLongOperation.StartOperation(this, () =>
                        {
                            if (string.IsNullOrEmpty(voidReason))
                            {
                                throw new PXSetPropertyException(
                                    string.Format(Messages.FieldRequired, "Void Reason"),
                                    typeof(VoidRequestFilter.voidReason).Name);
                            }
                            var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                            var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceESExt>();
                            var uploadGraph = CreateInstance<UploadFileMaintenance>();
                            var taskGraph = CreateInstance<CRTaskMaint>();

                            var envelope = GetEnvelopeInfo(maintenanceGraph, envelopeInfoId);
                            ESignAccount account =
                                fileMaintenanceExtension.ESignAccount.Search<ESignAccount.accountID>(
                                    envelope.ESignAccountID);

                            var actualEnvelope = fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                            if (!actualEnvelope.IsActionsAvailable.HasValue || !actualEnvelope.IsActionsAvailable.Value)
                            {
                                throw new PXException(Messages.EnvelopeVoidIsNotAvailable);
                            }

                            VerifyAccount(account);

                            switch (account.ProviderType)
                            {
                                case DocuSignProvider:
                                    VoidDocuSignEnvelope(account, envelope, voidReason);
                                    break;
                                case AdobeSignProvider:
                                    CancelAdobeSignEnvelope(account, actualEnvelope, voidReason);
                                    break;
                                default:
                                    throw new PXException(Messages.ProviderTypeIsMissing);
                            }

                            fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                        });
                    }
                }
            }
            return adapter.Get();
        }

        /// <summary>
        /// Send void request to <see cref="DocuSignService"/> service.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> SendVoidRequest;
        [PXUIField(DisplayName = Messages.Ok, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable sendVoidRequest(PXAdapter adapter)
        {
            return adapter.Get();
        }

        /// <summary>
        /// Send remind request to <see cref="DocuSignService"/> service.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> Remind;
        [PXButton(SpecialType = PXSpecialButtonType.Cancel)]
        [PXUIField(DisplayName = Messages.Remind, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable remind(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelopeInfoId = Envelopes.Current.EnvelopeInfoID;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceESExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();

                    var envelope = GetEnvelopeInfo(maintenanceGraph, envelopeInfoId);
                    var account = fileMaintenanceExtension.ESignAccount.SelectSingle(envelope.ESignAccountID);

                    VerifyAccount(account);
                    var actualEnvelope = fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                    if (!actualEnvelope.IsActionsAvailable.HasValue || !actualEnvelope.IsActionsAvailable.Value)
                    {
                        throw new PXException(Messages.EnvelopeRemindIsNotAvailable);
                    }

                    switch (account.ProviderType)
                    {
                        case DocuSignProvider:
                            RemindDocuSignEnvelope(account, actualEnvelope);
                            break;
                        case AdobeSignProvider:
                            RemindAdobeEnvelope(account, actualEnvelope);
                            break;
                        default:
                            throw new PXException(Messages.ProviderTypeIsMissing);
                    }
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Raise new window with ESign Iframe.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> ViewESign;
        [PXButton]
        [PXUIField(DisplayName = Messages.ViewESign, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable viewESign(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelope = Envelopes.Current;
                var account = ESignAccount.SelectSingle(envelope.ESignAccountID);
                PXLongOperation.StartOperation(this, () =>
                {
                    VerifyAccount(account);

                    switch (account.ProviderType)
                    {
                        case DocuSignProvider:
                            RedirectToDocuSign(account, envelope);
                            break;
                        case AdobeSignProvider:
                            RedirectToAdobeSign(account, envelope);
                            break;
                        default:
                            throw new PXException(Messages.ProviderTypeIsMissing);
                    }
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Send check status request to <see cref="WikiFileMaintenanceESExt"/> graph.
        /// </summary>
        public PXAction<ESignEnvelopeInfo> CheckStatus;
        [PXButton(SpecialType = PXSpecialButtonType.Refresh)]
        [PXUIField(DisplayName = Messages.CheckStatus, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable checkStatus(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelopeInfoId = Envelopes.Current.EnvelopeInfoID;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceESExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();

                    var envelope = GetEnvelopeInfo(maintenanceGraph, envelopeInfoId);
                    var account = fileMaintenanceExtension.ESignAccount.SelectSingle(envelope.ESignAccountID);

                    VerifyAccount(account);

                    fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                });
            }
            return adapter.Get();
        }

        #endregion

        #region Private methods

        private static void VerifyAccount(ESignAccount account)
        {
            if (account == null || account.IsActive != true)
            {
                throw new PXException(Messages.ESignAccountNotExists);
            }
        }

        private static void RedirectToAdobeSign(ESignAccount account, ESignEnvelopeInfo envelope)
        {
            var adobeSignClient = AdobeSignClientBuilder.Build(account);
            var url = ESignApiExecutor.TryExecute(() => adobeSignClient.DocumentsService
                .GetAgreementAssets(envelope.EnvelopeID), adobeSignClient).viewURL;
            throw new PXRedirectToUrlException(url, string.Empty);
        }

        private static void RedirectToDocuSign(ESignAccount account, ESignEnvelopeInfo envelope)
        {
            var dsService = new DocuSignService();
            var request = new BaseRequestModel
            {
                ESignAccount = account,
                EnvelopeId = envelope.EnvelopeID
            };

            var url = dsService.Redirect(request);
            throw new PXRedirectToUrlException(url.Url, string.Empty);
        }

        private static void VoidDocuSignEnvelope(ESignEnvelopeInfo envelope, ESignAccount account)
        {
            if (envelope.IsActionsAvailable != null
                && envelope.IsActionsAvailable.Value
                && envelope.EnvelopeInfoID.HasValue)
            {
                VoidDocuSignEnvelope(account, envelope, Messages.DefaultEnvelopeVoidReason);
            }
            else if (envelope.LastStatus == EnvelopeStatus.DocuSign.Created)
            {
                VoidDraftDocuSignEnvelope(account, envelope);
            }
        }

        public static void VoidAdobeSignEnvelope(ESignEnvelopeInfo envelope, ESignAccount account)
        {
            if (envelope.EnvelopeInfoID.HasValue)
            {
                var adobeSignClient = AdobeSignClientBuilder.Build(account);
                ESignApiExecutor.TryExecute(() => adobeSignClient.DocumentsService.DeleteAgreement(envelope.EnvelopeID),
                    adobeSignClient);
            }
        }

        public static void VoidDraftDocuSignEnvelope(ESignAccount account, ESignEnvelopeInfo envelope)
        {
            var dsService = new DocuSignService();
            var request = new VoidEnvelopeRequestModel
            {
                ESignAccount = account,
                EnvelopeId = envelope.EnvelopeID,
            };

            dsService.VoidDraftEnvelope(request);
        }

        public static void VoidDocuSignEnvelope(ESignAccount account, ESignEnvelopeInfo envelope, string voidReason)
        {
            var dsService = new DocuSignService();
            var request = new VoidEnvelopeRequestModel
            {
                ESignAccount = account,
                EnvelopeId = envelope.EnvelopeID,
                VoidReason = string.IsNullOrEmpty(voidReason) ? Messages.DefaultEnvelopeVoidReason : voidReason
            };

            dsService.VoidEnvelope(request);
        }

        public static void CancelAdobeSignEnvelope(ESignAccount account, ESignEnvelopeInfo actualEnvelope,
            string voidReason)
        {
            var adobeSignClient = AdobeSignClientBuilder.Build(account);
            ESignApiExecutor.TryExecute(
                () => adobeSignClient.DocumentsService.CancelAgreement(actualEnvelope.EnvelopeID, voidReason),
                adobeSignClient);
        }

        private static void RemindDocuSignEnvelope(ESignAccount account, ESignEnvelopeInfo actualEnvelope)
        {
            var dsService = new DocuSignService();
            var request = new BaseRequestModel
            {
                ESignAccount = account,
                EnvelopeId = actualEnvelope.EnvelopeID
            };

            dsService.RemindEnvelope(request);
        }

        private static void RemindAdobeEnvelope(ESignAccount account, ESignEnvelopeInfo actualEnvelope)
        {
            var adobeSignClient = AdobeSignClientBuilder.Build(account);
            var reminderModel = GetReminderCreationInfoModel(actualEnvelope);
            ESignApiExecutor.TryExecute(() => adobeSignClient.DocumentsService.SendReminder(reminderModel), adobeSignClient);
        }

        private static ReminderCreationInfoModel GetReminderCreationInfoModel(ESignEnvelopeInfo actualEnvelope)
        {
            return new ReminderCreationInfoModel
            {
                agreementId = actualEnvelope.EnvelopeID
            };
        }

        private static ESignEnvelopeInfo GetEnvelopeInfo(WikiFileMaintenance maintenanceGraph, int? envelopeInfoId)
        {
            return new PXSelect<ESignEnvelopeInfo,
                Where<ESignEnvelopeInfo.envelopeInfoID,
                    Equal<Required<ESignEnvelopeInfo.envelopeInfoID>>>>(maintenanceGraph)
                .SelectSingle(envelopeInfoId);
        }
    }

    #endregion
}
