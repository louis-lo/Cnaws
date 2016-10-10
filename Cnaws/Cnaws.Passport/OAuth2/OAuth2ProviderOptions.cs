using System;
using System.Collections.Generic;

namespace Cnaws.Passport.OAuth2
{
    public enum OAuth2GrantType
    {
        authorization_code,
        refresh_token
    }

    public sealed class OAuth2ProviderOptions
    {
        private List<string> _scope;

        public OAuth2ProviderOptions()
        {
            GrantType = OAuth2GrantType.authorization_code;
            _scope = new List<string>();
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Callback { get; set; }
        public string Secret { get; set; }
        public List<string> Scope { get { return _scope; } }
        public string RedirectUri { get; set; }
        public OAuth2GrantType GrantType { get; set; }
    }
}
