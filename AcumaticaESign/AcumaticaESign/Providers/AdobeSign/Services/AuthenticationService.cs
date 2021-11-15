using RestSharp;

namespace AcumaticaESign
{
    public class AuthenticationService : BaseApiService
    {
        public AuthenticationService(RequestDependencies dependencies) : base(dependencies)
        {
        }

        public string GetLoginPageUrl()
        {
            return Dependencies.Credentials.ApiUrl + "/public/oauth/v2" +
                   "?redirect_uri=" + Dependencies.Credentials.RedirectUrl +
                   "&response_type=code" +
                   "&client_id=" + Dependencies.Credentials.ClientId +
                   "&scope=user_login:self+agreement_send:self+agreement_write+agreement_read:self" +
                   "&state=" + Dependencies.Credentials.AccountId + "_" + Dependencies.Credentials.CompanyId;
        }

        public AccessTokenEntity CreateAccessToken(string authorizationCode)
        {
            return new RequestBuilder(Dependencies, Method.POST, "oauth/v2/token")
                .AddFormContentTypeHeader()
                .AddQueryParameter("code", authorizationCode)
                .AddQueryParameter("client_id", Dependencies.Credentials.ClientId)
                .AddQueryParameter("client_secret", Dependencies.Credentials.ClientSecret)
                .AddQueryParameter("redirect_uri", Dependencies.Credentials.RedirectUrl)
                .AddQueryParameter("grant_type", "authorization_code")
                .Execute<AccessTokenEntity>();
        }

        public AccessTokenEntity RefreshAccessToken()
        {
            return new RequestBuilder(Dependencies, Method.POST, "/oauth/v2/refresh")
                .AddFormContentTypeHeader()
                .AddQueryParameter("grant_type", "refresh_token")
                .AddQueryParameter("client_id", Dependencies.Credentials.ClientId)
                .AddQueryParameter("client_secret", Dependencies.Credentials.ClientSecret)
                .AddQueryParameter("refresh_token", Dependencies.Credentials.RefreshToken)
                .Execute<AccessTokenEntity>();
        }

        protected override void InitRestClient()
        {
            var api = string.IsNullOrEmpty(Dependencies.Credentials.ApiAccessPoint)
                ? Dependencies.Credentials.ApiUrl
                : Dependencies.Credentials.ApiAccessPoint;
            Dependencies.RestClient = new RestClient(api);
        }
    }
}
