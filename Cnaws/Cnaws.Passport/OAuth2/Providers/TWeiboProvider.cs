using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class TWeiboProvider : OAuth2Provider
    {
        public TWeiboProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.tweibo; }
        }
        public override string Name
        {
            get { return "腾讯微博"; }
        }
        public override string UidKey
        {
            get { return "openid"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://open.t.qq.com/cgi-bin/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://open.t.qq.com/cgi-bin/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(5);
            dict.Add("access_token", token.AccessToken);
            dict.Add("oauth_consumer_key", _options.ClientId);
            dict.Add("openid", token.UserId);
            dict.Add("clientip", HttpContext.Current.Request.UserHostAddress);
            dict.Add("oauth_version", "2.a");
            string url = "https://open.t.qq.com/api/user/info?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("ret"))
                throw new OAuth2Exception(500, json);
            JsonObject data = user["data"] as JsonObject;
            if (data == null)
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.tweibo,
                UserId = data["openid"] as JsonString,
                ScreenName = data["nick"] as JsonString,
                UserName = data["name"] as JsonString,
                Location = "",
                Description = data["introduction"] as JsonString,
                Image = (data["head"] as JsonString).Value + "/100",
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
