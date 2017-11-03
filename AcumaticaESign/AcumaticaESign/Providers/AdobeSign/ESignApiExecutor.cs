using System;
using PX.Data;

namespace AcumaticaESign
{
    internal class ESignApiExecutor
    {
        public static T TryExecute<T>(Func<T> method, AdobeSignClient client)
        {
            try
            {
                return method();
            }
            catch (AdobeSignUnauthorizedException)
            {
                try
                {
                    ResetRefreshToken(client);

                    return method();
                }
                catch (AdobeSignUnauthorizedException)
                {
                    DisconnectProcoreClient(client);
                    throw new PXException("");
                }
            }
        }

        public static T TryExecute<T, TArgs>(Func<TArgs, T> method, TArgs args, AdobeSignClient client)
        {
            try
            {
                return method(args);
            }
            catch (AdobeSignUnauthorizedException)
            {
                try
                {
                    ResetRefreshToken(client);
                    return method(args);
                }
                catch (AdobeSignUnauthorizedException)
                {
                    DisconnectProcoreClient(client);
                    throw new PXException();
                }
            }
        }

        private static void DisconnectProcoreClient(AdobeSignClient client)
        {
            var graph = PXGraph.CreateInstance<ESignAccountEntry>();
            var accountId = client.Authentication.Dependencies.Credentials.AccountId.Value;
            graph.RefreshCurrentSetup(accountId);
            graph.disconnect();
        }

        private static void ResetRefreshToken(AdobeSignClient client)
        {
            var accessToken = client.Authentication.RefreshAccessToken();
            client.Authentication.Dependencies.Credentials.AccessToken = accessToken.access_token;

            var graph = PXGraph.CreateInstance<ESignAccountEntry>();
            var accountId = client.Authentication.Dependencies.Credentials.AccountId.Value;
            graph.RefreshCurrentSetup(accountId);

            var setup = graph.Accounts.Current;
            setup.AccessToken = accessToken.access_token;

            graph.Accounts.Update(setup);
            graph.Actions.PressSave();
        }
    }
}
