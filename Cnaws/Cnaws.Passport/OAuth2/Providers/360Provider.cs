using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class _360Provider : OAuth2Provider
    {
        public _360Provider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType._360; }
        }
        public override string Name
        {
            get { return "360"; }
        }
        public override string UidKey
        {
            get { return "id"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://openapi.360.cn/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://openapi.360.cn/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(1);
            dict.Add("access_token", token.AccessToken);
            string url = "https://openapi.360.cn/user/me?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType._360,
                UserId = user["id"] as JsonString,
                ScreenName = user["name"] as JsonString,
                UserName = "",
                Location = "",
                Description = "",
                Image = user["avatar"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
