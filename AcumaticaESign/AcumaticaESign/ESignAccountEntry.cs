using Microsoft.AspNet.SignalR.Infrastructure;
using PX.Data;
using PX.Data.DependencyInjection;
using PX.OAuthClient.Hubs;
using PX.Objects.Common;

namespace AcumaticaESign
{
    public class ESignAccountEntry : PXGraph<ESignAccountEntry, ESignAccount>, IGraphWithInitialization
    {
        [InjectDependency]
        private IConnectionManager _signalRConnectionManager { get; set; }

        public void Initialize()
        {
        }

        #region Selects

        public PXSelect<ESignAccount> Accounts;

        public PXSelect<ESignAccount,
            Where<ESignAccount.accountID, Equal<Current<ESignAccount.accountID>>>> SelectedAccount;

        public PXSelect<ESignAccountUserRule,
            Where<ESignAccountUserRule.accountID, Equal<Current<ESignAccountUserRule.accountID>>>> Users;

        #endregion

        #region Actions

        public PXAction<ESignAccount> Connect;
        [PXButton]
        [PXUIField(DisplayName = Messages.Actions.Connect, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Select)]
        public virtual void connect()
        {
            Actions.PressSave();
            var account = Accounts.Current;

            switch (account.ProviderType)
            {
                case ESignAccount.ProviderTypes.DocuSign:
                    AuthenticateDocuSignAccount(account);
                    break;
                case ESignAccount.ProviderTypes.AdobeSign:
                    AuthenticateAdobeSignAccount(account);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }
        }

        public PXAction<ESignAccount> Disconnect;
        [PXButton]
        [PXUIField(DisplayName = Messages.Actions.Disconnect, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Select)]
        public virtual void disconnect()
        {
            var account = Accounts.Current;
            account.AccessToken = null;
            account.Status = Messages.ESignIntegrationStatus.Disconnected;

            Accounts.Update(account);
            Actions.PressSave();
        }

        public void RefreshCurrentSetup(int accountId)
        {
            Accounts.Current =
                new PXSelect<ESignAccount, Where<ESignAccount.accountID, Equal<Required<ESignAccount.accountID>>>>(this)
                    .Select(accountId);
        }

        public void SetAdobeApiUrl(string apiUrl)
        {
            var account = Accounts.Current;
            account.ApiAccessPoint = apiUrl;

            Accounts.Update(account);
            Actions.PressSave();
        }

        public void GetAccessToken(string code)
        {
            var account = Accounts.Current;

            switch (account.ProviderType)
            {
                case ESignAccount.ProviderTypes.DocuSign:
                    break;
                case ESignAccount.ProviderTypes.AdobeSign:
                    GetAdobeAccessToken(code, account);
                    break;
                default:
                    throw new PXException(Messages.ProviderTypeIsMissing);
            }

            SendRefreshCall();
        }

        private void GetAdobeAccessToken(string code, ESignAccount account)
        {
            var adobeSignClient = AdobeSignClientBuilder.BuildUnauthorized(Accounts.Current);
            var accessToken = ESignApiExecutor.TryExecute(adobeSignClient.Authentication.CreateAccessToken, code, adobeSignClient);
            account.AccessToken = accessToken.access_token;
            account.RefreshToken = accessToken.refresh_token;
            account.Status = Messages.ESignIntegrationStatus.Connected;

            Accounts.Update(account);
            Actions.PressSave();
        }

        #endregion

        #region Events

        protected virtual void _(Events.FieldVerifying<ESignAccount.type> args)
        {
            var account = args.Row as ESignAccount;
            if (account == null)
            {
                return;
            }

            if ((string)args.NewValue == ESignAccount.AccountTypes.Shared)
            {
                var result = PXSelect<ESignAccount,
                    Where<ESignAccount.type, Equal<Required<ESignAccount.type>>,
                        And<ESignAccount.providerType, Equal<Required<ESignAccount.providerType>>>>>.Select(this, args.NewValue, account.ProviderType);
                if (result.Count > 0)
                {
                    throw new PXSetPropertyException(Messages.SharedAccountExists);
                }
            }
        }

        protected virtual void _(Events.RowPersisting<ESignAccount> args)
        {
            var account = args.Row as ESignAccount;
            if (account == null)
            {
                return;
            }

            if (account.SendReminders == true && string.IsNullOrEmpty(account.ReminderType))
            {
                args.Cache.RaiseExceptionHandling<ESignAccount.reminderType>(account, account.ReminderType,
                    new PXSetPropertyException<ESignAccount.reminderType>(
                        string.Format(Messages.FieldRequired, Messages.ESignAccount.RemindersType)));
                args.Cancel = true;
            }

            if ((args.Operation & PXDBOperation.Command) != PXDBOperation.Delete
                && (account.Type == ESignAccount.AccountTypes.Shared && !Users.Any()))
            {
                throw new PXRowPersistingException(
                    typeof(ESignAccountUserRule).Name,
                    null,
                    Messages.ESignAccountSharedNotNullUsers);
            }
        }

        protected virtual void _(Events.RowSelected<ESignAccount> args)
        {
            var account = args.Row;
            if (account == null)
            {
                return;
            }

            Users.Cache.AllowSelect = account.Type.Equals(ESignAccount.AccountTypes.Shared);
            PXUIFieldAttribute.SetVisible<ESignAccount.ownerID>(Accounts.Cache, null, !Users.Cache.AllowSelect);

            if (account.Type != ESignAccount.AccountTypes.Individual)
            {
                PXDefaultAttribute.SetPersistingCheck<ESignAccount.ownerID>(Accounts.Cache, null, PXPersistingCheck.Nothing);
            }

            var isAdobe = account.ProviderType == ESignAccount.ProviderTypes.AdobeSign;

            PXUIFieldAttribute.SetVisible<ESignAccount.reminderType>(Accounts.Cache, null, isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.clientID>(Accounts.Cache, null, isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.clientSecret>(Accounts.Cache, null, isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.warnDays>(Accounts.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.firstReminderDay>(Accounts.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.reminderFrequency>(Accounts.Cache, null, !isAdobe);

            PXUIFieldAttribute.SetVisible<ESignAccount.isTestApi>(Accounts.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.email>(Accounts.Cache, null, !isAdobe);
            PXUIFieldAttribute.SetVisible<ESignAccount.password>(Accounts.Cache, null, !isAdobe);

            var isApiUrlRequired = isAdobe || account.IsTestApi == true;
            var apiUrlVisibility = isApiUrlRequired
                ? PXPersistingCheck.NullOrBlank
                : PXPersistingCheck.Nothing;
            PXDefaultAttribute.SetPersistingCheck<ESignAccount.apiUrl>(Accounts.Cache, account, apiUrlVisibility);
            PXUIFieldAttribute.SetRequired<ESignAccount.apiUrl>(Accounts.Cache, isApiUrlRequired);

            if (account.IsTestApi != null)
            {
                PXUIFieldAttribute.SetEnabled<ESignAccount.apiUrl>(Accounts.Cache, null, account.IsTestApi.Value);
            }

            if (account.SendReminders != null)
            {
                PXUIFieldAttribute.SetEnabled<ESignAccount.firstReminderDay>(Accounts.Cache, null, account.SendReminders.Value);
                PXUIFieldAttribute.SetEnabled<ESignAccount.reminderFrequency>(Accounts.Cache, null, account.SendReminders.Value);
                PXUIFieldAttribute.SetEnabled<ESignAccount.reminderType>(Accounts.Cache, null, account.SendReminders.Value);
            }

            var isDisconnected = IsDisconnected(account) || !isAdobe;
            Disconnect.SetVisible(isAdobe);
            Connect.SetEnabled(isDisconnected);
            Disconnect.SetEnabled(!isDisconnected);

            PXUIFieldAttribute.SetEnabled<ESignAccount.clientID>(Accounts.Cache, null, isDisconnected);
            PXUIFieldAttribute.SetEnabled<ESignAccount.clientSecret>(Accounts.Cache, null, isDisconnected);
            PXUIFieldAttribute.SetEnabled<ESignAccount.apiUrl>(Accounts.Cache, null,
                isDisconnected && isApiUrlRequired);
            PXUIFieldAttribute.SetEnabled<ESignAccount.providerType>(Accounts.Cache, null, isDisconnected);
            PXUIFieldAttribute.SetVisible<ESignAccount.status>(Accounts.Cache, null, isAdobe);
        }

        protected virtual void _(Events.FieldUpdated<ESignAccount.providerType> args)
        {
            var account = args.Row as ESignAccount;
            if (account != null && account.ProviderType == ESignAccount.ProviderTypes.DocuSign)
            {
                account.ApiUrl = null;
            }
        }

        protected virtual void _(Events.FieldUpdated<ESignAccount.isTestApi> args)
        {
            var account = args.Row as ESignAccount;

            if (account != null)
            {
                account.ApiUrl = null;
            }
        }

        protected virtual void _(Events.FieldUpdated<ESignAccount.type> args)
        {
            var account = args.Row as ESignAccount;
            if (account == null)
            {
                return;
            }

            if (account.Type.Equals(ESignAccount.AccountTypes.Shared))
            {
                account.OwnerID = null;
            }

            var users = Users.Select(account.AccountID);
            foreach (var user in users)
            {
                Users.Delete(user);
            }
        }

        protected virtual void _(Events.FieldUpdated<ESignAccount.ownerID> args)
        {
            var account = args.Row as ESignAccount;
            if (account == null)
            {
                return;
            }
            var users = Users.Select(account.AccountID);
            foreach (var user in users)
            {
                Users.Delete(user);
            }
        }

        #endregion

        #region Private Functions

        private void SendRefreshCall()
        {
            var hubContext = _signalRConnectionManager.GetHubContext<RefreshHub>();
            hubContext.Clients.All.RefreshPage();
        }

        private static bool IsDisconnected(ESignAccount account)
        {
            return account == null || account.Status == Messages.ESignIntegrationStatus.Disconnected;
        }

        private void AuthenticateAdobeSignAccount(ESignAccount account)
        {
            var companyId = PXAccess.GetCompanyName();
            var client = AdobeSignClientBuilder.BuildUnauthorized(account, companyId);
            PXLongOperation.StartOperation(this, () =>
            {
                var loginUrl = client.Authentication.GetLoginPageUrl();
                throw new PXRedirectToUrlException(loginUrl, PXBaseRedirectException.WindowMode.InlineWindow,
                    string.Empty, false);
            });
        }

        private void AuthenticateDocuSignAccount(ESignAccount account)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                DocuSignService.Authenticate(account);
                account.Status = Messages.ESignIntegrationStatus.Connected;
                Accounts.Update(account);
                Actions.PressSave();
            });
        }

        #endregion
    }
}
