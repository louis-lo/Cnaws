using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class SohuProvider : OAuth2Provider
    {
        public SohuProvider(OAuth2ProviderOptions options)
            : base(options)
        {
            if (options.Scope.Count == 0)
                options.Scope.Add("basic");
            _scope = options.Scope;
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.sohu; }
        }
        public override string Name
        {
            get { return "搜狐微博"; }
        }
        public override string UidKey
        {
            get { return "id"; }
        }
        public override string StateKey
        {
            get { return "wrap_client_state"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://api.t.sohu.com/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://api.t.sohu.com/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(1);
            dict.Add("access_token", token.AccessToken);
            string url = "http://api.t.sohu.com/users/show/id.json?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.sohu,
                UserId = user["id"] as JsonString,
                ScreenName = user["screen_name"] as JsonString,
                UserName = user["name"] as JsonString,
                Location = "",
                Description = user["description"] as JsonString,
                Image = user["profile_image_url"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
