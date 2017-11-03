using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Newtonsoft.Json;
using PX.Common;
using PX.Data;
using Doc = DocuSign.eSign.Model.Document;
using Notification = DocuSign.eSign.Model.Notification;
using Configuration = DocuSign.eSign.Client.Configuration;

namespace AcumaticaESign
{
    /// <summary>
    /// Implements communication logic between acumatica and docusign.
    /// Use <see cref="EnvelopesApi"/>rest api to send request.
    /// </summary>
    internal class DocuSignService
    {
        private const string IntegratorKey = "[Your Integration Key]";

        /// <summary>
        /// Used for creating uuthentication request for docusign.
        /// <param name="account">Docusign account<see cref="DocuSignAccount"></see> </param>
        /// </summary>
        public static string Authenticate(ESignAccount account)
        {
            var model = new BaseRequestModel { ESignAccount = account };
            return TryExecute(AuthenticateFunc(), model);
        }

        /// <summary>
        /// Used for creating uuthentication request for docusign.
        /// <param name="account">Docusign account<see cref="ESignAccount"></see> </param>
        /// </summary>
        public static LoginAccount AuthenticateUser(ESignAccount account)
        {
            var model = new BaseRequestModel { ESignAccount = account };
            return TryExecute(AuthenticateUserFunc(), model);
        }

        /// <summary>
        /// Used for creating request for new envelope.
        /// <param name="request">Instance of the<see cref="CreateEnvelopeRequestModel"></see></param>
        /// </summary>
        public CreateEnvelopeResponseModel CreateEnvelope(CreateEnvelopeRequestModel request)
        {
            return TryExecute(SendRequestFunc(), request);
        }

        /// <summary>
        /// Used for creating request for updating envelope.
        /// <param name="request">Instance of the<see cref="CreateEnvelopeRequestModel"></see></param>
        /// </summary>
        public UpdateEnvelopeResponseModel UpdateEnvelope(CreateEnvelopeRequestModel request)
        {
            return TryExecute(UpdateRequestFunc(), request);
        }

        /// <summary>
        /// Create ViewUrl for redirecting to documatica Iframe
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public ViewUrl Redirect(BaseRequestModel request)
        {
            return TryExecute(RedirectFunc(), request);
        }

        /// <summary>
        /// Check if file exists in th docusign.
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public bool IsFileExist(BaseRequestModel request)
        {
            return TryExecute(IsFileExistFunc(), request);
        }

        /// <summary>
        /// Used for creating request to get history of the envelope.
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public GetEnvelopeHistoryResponseModel GetEnvelopeHistory(BaseRequestModel request)
        {
            return TryExecute(GetEnvelopeHistoryFunc(), request);
        }

        /// <summary>
        /// Used for creating request for voiding envelope.
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public void VoidEnvelope(VoidEnvelopeRequestModel request)
        {
            TryExecute(VoidEnvelopeFunc(), request);
        }

        /// <summary>
        /// Used for creating request for voiding envelope.
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public void VoidDraftEnvelope(VoidEnvelopeRequestModel request)
        {
            TryExecute(VoidDraftEnvelopeFunc(), request);
        }

        /// <summary>
        /// Used for creating request for remind recipients of the envelope.
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public void RemindEnvelope(BaseRequestModel request)
        {
            TryExecute(RemindEnvelopeFunc(), request);
        }

        /// <summary>
        /// Used for retrieving document of the envelope
        /// <param name="request">Instance of the<see cref="BaseRequestModel"></see></param>
        /// </summary>
        public GetEnvelopeDocumentResponseModel GetEnvelopeDocument(BaseRequestModel request)
        {
            return TryExecute(GetEnvelopeDocumentFunc(), request);
        }

        #region Private fucntions

        private static T TryExecute<T, TArgs>(Func<TArgs, T> method, TArgs args)
            where TArgs : BaseRequestModel
        {
            try
            {
                return method(args);
            }
            catch (ApiException exception)
            {
                ApiErrorModel error = JsonConvert.DeserializeObject<ApiErrorModel>(exception.ErrorContent);

                if (error.errorCode == Constants.ErrorCode.DocuSignEnvelopeNotInFolder
                    || error.errorCode == Constants.ErrorCode.DocuSignEnvelopeVoidInvalidState)
                {
                    return default(T);
                }

                if (error.errorCode == Constants.ErrorCode.DocuSignEnvelopeLock)
                {
                    var accountId = Authenticate(args.ESignAccount);
                    var envelopesApi = new EnvelopesApi();

                    var lockEnvelope = envelopesApi.GetLock(accountId, args.EnvelopeId);
                    var time = PXTimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.Parse(lockEnvelope.LockedUntilDateTime).ToUniversalTime(),
                        LocaleInfo.GetTimeZone(),
                        false);

                    var message = string.Format(Messages.ESignEnvelopeLock, time.ToShortTimeString());
                    throw new PXException(message);
                }

                var errorMessage = ErrorCodeHelper.GetValueByKey(error.errorCode);
                if (errorMessage != null)
                {
                    throw new PXException(errorMessage);
                }

                throw new PXException(error.message);
            }
        }

        private static Func<BaseRequestModel, GetEnvelopeHistoryResponseModel> GetEnvelopeHistoryFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();

                var envelope = envelopesApi.GetEnvelope(accountId, request.EnvelopeId);
                var recipients = envelopesApi.ListRecipients(accountId, request.EnvelopeId);
                var notification = envelopesApi.GetNotificationSettings(accountId, envelope.EnvelopeId);
                var events = envelopesApi.ListAuditEvents(accountId, envelope.EnvelopeId);
                return new GetEnvelopeHistoryResponseModel
                {
                    Envelope = envelope,
                    Recipients = recipients,
                    Notification = notification,
                    Events = events,
                };
            };
        }

        private static Func<BaseRequestModel, GetEnvelopeDocumentResponseModel> GetEnvelopeDocumentFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var docList = envelopesApi.ListDocuments(accountId, request.EnvelopeId).EnvelopeDocuments;

                var docStream = (MemoryStream)envelopesApi.GetDocument(accountId, request.EnvelopeId, docList[0].DocumentId);
                docStream.Seek(0, SeekOrigin.Begin);

                var document = new byte[docStream.Capacity];
                docStream.Read(document, 0, docStream.Capacity);

                return new GetEnvelopeDocumentResponseModel
                {
                    Document = document
                };
            };
        }

        private Func<VoidEnvelopeRequestModel, UpdateEnvelopeResponseModel> VoidEnvelopeFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var envelope = new Envelope
                {
                    Status = EnvelopeStatus.DocuSign.Voided,
                    VoidedReason = request.VoidReason
                };
                var result = envelopesApi.Update(accountId, request.EnvelopeId, envelope);
                return new UpdateEnvelopeResponseModel
                {
                    Envelope = result
                };
            };
        }

        private Func<VoidEnvelopeRequestModel, UpdateEnvelopeResponseModel> VoidDraftEnvelopeFunc()
        {
            return request =>
            {
                var account = AuthenticateUser(request.ESignAccount);
                var folderApi = new FoldersApi();
                var folders = folderApi.List(account.AccountId);
                var deletedFolder = folders.Folders.FirstOrDefault(x => x.Type == "recyclebin" && x.OwnerUserId == account.UserId);

                var currentFolder = folders.Folders
                    .FirstOrDefault(folder => folderApi.ListItems(account.AccountId, folder.FolderId).FolderItems
                        .Any(folderItems => folderItems.EnvelopeId == request.EnvelopeId));

                if (deletedFolder != null && currentFolder != null)
                {
                    folderApi.MoveEnvelopes(account.AccountId, deletedFolder.FolderId, new FoldersRequest
                    {
                        EnvelopeIds = new List<string> { request.EnvelopeId },
                        FromFolderId = currentFolder.FolderId
                    });
                }

                return null;
            };
        }

        private Func<BaseRequestModel, UpdateEnvelopeResponseModel> RemindEnvelopeFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var options = new EnvelopesApi.UpdateOptions { resendEnvelope = true.ToString() };
                var result = envelopesApi.Update(accountId, request.EnvelopeId, new Envelope(), options);
                return new UpdateEnvelopeResponseModel
                {
                    Envelope = result
                };
            };
        }

        private static Func<BaseRequestModel, string> AuthenticateFunc()
        {
            return request =>
            {
                Configuration.Default.ApiClient = request.ESignAccount.IsTestApi.HasValue && request.ESignAccount.IsTestApi.Value
                    ? new ApiClient(request.ESignAccount.ApiUrl)
                    : new ApiClient();

                Configuration.Default.DefaultHeader.Clear();
                Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", GetAuthHeader(request.ESignAccount));
                var authApi = new AuthenticationApi();
                var loginInfo = authApi.Login();

                //For Production environment 
                /*
                 * As returned by login method, the baseUrl includes the API version and account id. Split the string to obtain the basePath, 
                 * just the server name and api name. Eg, you will receive https://na1.docusign.net/restapi/v2/accounts/123123123. 
                 * You want just https://na1.docusign.net/restapi
                 * Instantiate the SDK using the basePath. Eg ApiClient apiClient = new ApiClient(basePath);
                 */
                if (!request.ESignAccount.IsTestApi.HasValue || !request.ESignAccount.IsTestApi.Value)
                {
                    var baseURL = loginInfo.LoginAccounts.FirstOrDefault() != null ? loginInfo.LoginAccounts.FirstOrDefault().BaseUrl : null;
                    if (!string.IsNullOrEmpty(baseURL))
                    {
                        var sLookFor = "restapi";
                        baseURL = baseURL.Substring(0, baseURL.IndexOf(sLookFor) + sLookFor.Length);
                        Configuration.Default.ApiClient = new ApiClient(baseURL);
                    }
                }

                return loginInfo.LoginAccounts
                    .Where(x => x.IsDefault == true.ToString())
                    .Select(x => x.AccountId).FirstOrDefault() ?? loginInfo.LoginAccounts[0].AccountId;
            };
        }

        private static Func<BaseRequestModel, LoginAccount> AuthenticateUserFunc()
        {
            return request =>
            {
                Configuration.Default.ApiClient = request.ESignAccount.IsTestApi.HasValue && request.ESignAccount.IsTestApi.Value
                    ? new ApiClient(request.ESignAccount.ApiUrl)
                    : new ApiClient();

                Configuration.Default.DefaultHeader.Clear();
                Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", GetAuthHeader(request.ESignAccount));
                var authApi = new AuthenticationApi();
                var loginInfo = authApi.Login();

                //For Production environment 
                /*
                 * As returned by login method, the baseUrl includes the API version and account id. Split the string to obtain the basePath, 
                 * just the server name and api name. Eg, you will receive https://na1.docusign.net/restapi/v2/accounts/123123123. 
                 * You want just https://na1.docusign.net/restapi
                 * Instantiate the SDK using the basePath. Eg ApiClient apiClient = new ApiClient(basePath);
                 */
                if (!request.ESignAccount.IsTestApi.HasValue || !request.ESignAccount.IsTestApi.Value)
                {
                    var baseURL = loginInfo.LoginAccounts.FirstOrDefault() != null ? loginInfo.LoginAccounts.FirstOrDefault().BaseUrl : null;
                    if (!string.IsNullOrEmpty(baseURL))
                    {
                        var sLookFor = "restapi";
                        baseURL = baseURL.Substring(0, baseURL.IndexOf(sLookFor) + sLookFor.Length);
                        Configuration.Default.ApiClient = new ApiClient(baseURL);
                    }
                }

                return loginInfo.LoginAccounts.FirstOrDefault(x => x.IsDefault == true.ToString()) ?? loginInfo.LoginAccounts[0];
            };
        }

        private Func<CreateEnvelopeRequestModel, CreateEnvelopeResponseModel> SendRequestFunc()
        {
            return request =>
            {
                var envDef = CreateEnvelopeDefinition(GetDocument(request), request);

                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var options = new ReturnUrlRequest { ReturnUrl = string.Empty };

                var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);
                var senderView = envelopesApi.CreateSenderView(accountId, envelopeSummary.EnvelopeId, options);

                return new CreateEnvelopeResponseModel
                {
                    EnvelopeSummary = envelopeSummary,
                    ViewUrl = senderView
                };
            };
        }

        private Func<BaseRequestModel, ViewUrl> RedirectFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var isFileExist = IsFileExist(request);
                var options = new ReturnUrlRequest
                {
                    ReturnUrl = request.ESignAccount.IsTestApi != null && request.ESignAccount.IsTestApi.Value
                        ? Constants.Url.DemoDocuSignDocumentDetailsUrl + request.EnvelopeId
                        : Constants.Url.DocuSignDocumentDetailsUrl + request.EnvelopeId
                };
                if (isFileExist)
                {
                    return envelopesApi.CreateSenderView(accountId, request.EnvelopeId, options);
                }
                throw new PXException(Messages.ESignEnvelopeNotExists);
            };
        }

        private static Func<BaseRequestModel, bool> IsFileExistFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var folderApi = new FoldersApi();
                var folders = folderApi.List(accountId);

                return folders.Folders
                    .Select(folder => folderApi.ListItems(accountId, folder.FolderId))
                    .Any(listItems => listItems.FolderItems
                        .Any(listItem => listItem.EnvelopeId == request.EnvelopeId));
            };
        }

        private Func<CreateEnvelopeRequestModel, UpdateEnvelopeResponseModel> UpdateRequestFunc()
        {
            return request =>
            {
                var accountId = Authenticate(request.ESignAccount);
                var envelopesApi = new EnvelopesApi();
                var options = new ReturnUrlRequest { ReturnUrl = string.Empty };

                //VerifyEnvelopeLock(envelopesApi, accountId, request);

                UpdateRecipients(request, envelopesApi, accountId);
                UpdateDocumets(request, envelopesApi, accountId);

                var envelope = envelopesApi.GetEnvelope(accountId, request.EnvelopeInfo.EnvelopeID);

                var updatedEnvelope = new Envelope();
                envelope.Notification = CreateNotification(request.EnvelopeInfo);
                envelope.EmailSubject = request.EnvelopeInfo.Theme;
                envelope.EmailBlurb = request.EnvelopeInfo.MessageBody;

                var result = envelopesApi.Update(accountId, request.EnvelopeInfo.EnvelopeID, updatedEnvelope);
                var senderView = envelopesApi.CreateSenderView(accountId, request.EnvelopeInfo.EnvelopeID, options);

                return new UpdateEnvelopeResponseModel
                {
                    Envelope = result,
                    ViewUrl = senderView
                };
            };
        }

        private static void UpdateDocumets(CreateEnvelopeRequestModel request, IEnvelopesApi envelopesApi, string accountId)
        {
            var list = envelopesApi.ListDocuments(accountId, request.EnvelopeInfo.EnvelopeID);
            var def = new EnvelopeDefinition { Documents = new List<Doc>() };
            foreach (var doc in list.EnvelopeDocuments)
            {
                def.Documents.Add(new Doc { DocumentId = doc.DocumentId });
            }

            envelopesApi.DeleteDocuments(accountId, request.EnvelopeInfo.EnvelopeID, def);

            def.Documents = new List<Doc> { GetDocument(request) };
            envelopesApi.UpdateDocuments(accountId, request.EnvelopeInfo.EnvelopeID, def);
        }

        private static void UpdateRecipients(CreateEnvelopeRequestModel request, IEnvelopesApi envelopesApi, string accountId)
        {
            var exisitingResipients = envelopesApi.ListRecipients(accountId, request.EnvelopeInfo.EnvelopeID);
            envelopesApi.DeleteRecipients(accountId, request.EnvelopeInfo.EnvelopeID, exisitingResipients);
            var newRecipients = new Recipients
            {
                CarbonCopies = ParseCarbonCopy(request.CarbonRecipients, request.EnvelopeInfo).ToList(),
                Signers = ParseSigners(request.Recipients, request.EnvelopeInfo).ToList()
            };
            envelopesApi.UpdateRecipients(accountId, request.EnvelopeInfo.EnvelopeID, newRecipients);
        }


        private static string GetAuthHeader(ESignAccount account)
        {
            return $"<DocuSignCredentials>" +
                   $"<Username>{account.Email}</Username>" +
                   $"<Password>{account.Password}</Password>" +
                   $"<IntegratorKey>{IntegratorKey}</IntegratorKey>" +
                   $"</DocuSignCredentials>";
        }

        private static Doc GetDocument(CreateEnvelopeRequestModel request)
        {
            var fileExtension = request.FileInfo.OriginalName
                .Substring(request.FileInfo.OriginalName
                .IndexOf(".", StringComparison.Ordinal) + 1);
            return new Doc
            {
                DocumentBase64 = Convert.ToBase64String(request.FileInfo.BinData),
                Name = request.FileInfo.FullName,
                DocumentId = request.EnvelopeInfo.EnvelopeInfoID.ToString(),
                FileExtension = fileExtension
            };
        }

        private static EnvelopeDefinition CreateEnvelopeDefinition(Doc document, CreateEnvelopeRequestModel request)
        {
            return new EnvelopeDefinition
            {
                Documents = new List<Doc> { document },
                EmailSubject = request.EnvelopeInfo.Theme,
                EmailBlurb = request.EnvelopeInfo.MessageBody,
                Status = EnvelopeStatus.DocuSign.Created,
                Notification = CreateNotification(request.EnvelopeInfo),
                Recipients = new Recipients
                {
                    CarbonCopies = ParseCarbonCopy(request.CarbonRecipients, request.EnvelopeInfo).ToList(),
                    Signers = ParseSigners(request.Recipients, request.EnvelopeInfo).ToList()
                }
            };
        }

        private static Notification CreateNotification(ESignEnvelopeInfo envelope)
        {
            var reminder = new Reminders();

            if (envelope.SendReminders != false)
            {
                reminder = new Reminders
                {
                    ReminderDelay = envelope.FirstReminderDay.ToString(),
                    ReminderFrequency = envelope.ReminderFrequency.ToString(),
                    ReminderEnabled = true.ToString()
                };
            }

            return new Notification
            {
                UseAccountDefaults = false.ToString(),
                Reminders = reminder,
                Expirations = new Expirations
                {
                    ExpireAfter = envelope.ExpiredDays.ToString(),
                    ExpireWarn = envelope.WarnDays.ToString(),
                    ExpireEnabled = true.ToString()
                }
            };
        }

        private static IEnumerable<CarbonCopy> ParseCarbonCopy(IEnumerable<ESignRecipient> recipients, ESignEnvelopeInfo envelopeInfo)
        {
            return recipients.Select(item => new CarbonCopy
            {
                Email = item.Email,
                Name = item.Name,
                RecipientId = item.RecipientID.ToString(),
                Note = item.CustomMessage,
                RoutingOrder = envelopeInfo.IsOrder.HasValue && envelopeInfo.IsOrder.Value
                    ? item.Position.ToString()
                    : 1.ToString()
            });
        }

        private static IEnumerable<Signer> ParseSigners(IEnumerable<ESignRecipient> recipients, ESignEnvelopeInfo envelopeInfo)
        {
            var tabs = new Tabs();
            return recipients.Select(item => new Signer
            {
                Name = item.Name,
                Email = item.Email,
                RecipientId = item.RecipientID.ToString(),
                Tabs = tabs,
                Note = item.CustomMessage,
                RoutingOrder = envelopeInfo.IsOrder.HasValue && envelopeInfo.IsOrder.Value
                    ? item.Position.ToString()
                    : 1.ToString()
            });
        }

        #endregion
    }
}
