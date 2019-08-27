using System;
using System.Collections.Concurrent;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SharePointCSOMAPI
{
    class TokenProvider
    {
        private ConcurrentDictionary<string, AuthenticationResult> tokenCache;

        public TokenProvider()
        {
            tokenCache = new ConcurrentDictionary<string, AuthenticationResult>();
        }

        public AuthenticationResult GetAccessToken(string resource)
        {
            lock (tokenCache)
            {
                if (tokenCache.ContainsKey(resource) && !IsExpire(tokenCache[resource]))
                {
                    return tokenCache[resource];
                }
                if (tokenCache.Count == 0)
                {
                    tokenCache[resource] = GetAccessToken(Configuration.Config.Authority, resource, Configuration.Config.ClientId, Configuration.Config.RedirectURL, PromptBehavior.SelectAccount, Configuration.Config.ExtraQuery);
                }
                else
                {
                    tokenCache[resource] = GetAccessToken(Configuration.Config.Authority, resource, Configuration.Config.ClientId, Configuration.Config.RedirectURL, PromptBehavior.Auto, Configuration.Config.ExtraQuery);
                }
            }
            return tokenCache[resource];
        }

        private AuthenticationResult GetAccessToken(string authorityUrl, string resource, string clientId, string redirectUrl, PromptBehavior promptBehavior, string extraQuery)
        {
            AuthenticationContext context = new AuthenticationContext(authorityUrl, false);
            return context.AcquireTokenAsync(resource,
                clientId,
                new System.Uri(redirectUrl),
                new PlatformParameters(promptBehavior),
                UserIdentifier.AnyUser,
                extraQuery).Result;
        }

        private bool IsExpire(AuthenticationResult token)
        {
            if ((token.ExpiresOn.UtcDateTime - DateTime.UtcNow).Minutes <= 5)
            {
                return true;
            }
            return false;
        }
    }
}
