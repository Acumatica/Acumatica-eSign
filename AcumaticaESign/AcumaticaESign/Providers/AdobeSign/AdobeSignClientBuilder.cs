﻿using System;
using System.Text.RegularExpressions;
using System.Web;

namespace AcumaticaESign
{
    internal static class AdobeSignClientBuilder
    {
        public static AdobeSignClient Build(ESignAccount account, string companyId = null)
        {
            var credentials = CreateCredentials(account, companyId);
            return new AdobeSignClient(credentials);
        }

        public static AdobeSignClient BuildUnauthorized(ESignAccount account, string companyId = null)
        {
			string leftPart = IsSecureConnection() && HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).StartsWith("http:", StringComparison.OrdinalIgnoreCase) ?
				Regex.Replace(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), "http:", "https:", RegexOptions.IgnoreCase) :
				HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            var credentials = CreateCredentials(account, companyId);
            if (HttpContext.Current != null)
            {
                var url = string.Concat(
                    leftPart,
                    HttpContext.Current.Request.ApplicationPath != null && HttpContext.Current.Request.ApplicationPath.EndsWith("/")
                            ? string.Concat(HttpContext.Current.Request.ApplicationPath, "Pages/ES/ESign.aspx")
                            : string.Concat(HttpContext.Current.Request.ApplicationPath, "/", "Pages/ES/ESign.aspx"));
                credentials.RedirectUrl = url;
            }
            return new AdobeSignClient(credentials);
        }

        private static Credentials CreateCredentials(ESignAccount setup, string companyId = null)
        {
            return new Credentials
            {
                AccountId = setup.AccountID,
                ApiUrl = setup.ApiUrl,
                ClientId = setup.ClientID,
                ClientSecret = setup.ClientSecret,
                ApiAccessPoint = setup.ApiAccessPoint,
                AccessToken = setup.AccessToken,
                RefreshToken = setup.RefreshToken,
                CompanyId = companyId
            };
        }

		private static bool IsSecureConnection()
		{
			return
				HttpContext.Current.Request.IsSecureConnection
				|| string.Equals(HttpContext.Current.Request.Headers["X-Forwarded-Proto"],
					"https",
					StringComparison.InvariantCultureIgnoreCase);
		}
    }
}
