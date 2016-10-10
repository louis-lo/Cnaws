using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class FeixinProvider : OAuth2Provider
    {
        public FeixinProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.feixin; }
        }
        public override string Name
        {
            get { return "飞信"; }
        }
        public override string UidKey
        {
            get { return "userId"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://i.feixin.10086.cn/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://i.feixin.10086.cn/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(1);
            dict.Add("access_token", token.AccessToken);
            string url = "https://i.feixin.10086.cn/api/user.json?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.feixin,
                UserId = user["userId"] as JsonString,
                ScreenName = user["nickname"] as JsonString,
                UserName = "",
                Location = "",
                Description = user["introducation"] as JsonString,
                Image = user["portraitTiny"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
