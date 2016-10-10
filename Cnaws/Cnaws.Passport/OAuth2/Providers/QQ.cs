using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class QQ : OAuth2Provider
    {
        public QQ(OAuth2ProviderOptions options)
            : base(options)
        {
        }
        
        public override string Name
        {
            get { return "QQ"; }
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
            get { return "https://graph.qq.com/oauth2.0/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://graph.qq.com/oauth2.0/token"; }
        }

        public override OAuth2Member GetUserInfo(OAuth2TokenAccess token)
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            dict.Add("access_token", token.AccessToken);
            string url = "https://graph.qq.com/oauth2.0/me?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            if (json.IndexOf("callback") > -1)
            {
                int lpos = json.IndexOf('(');
                int rpos = json.LastIndexOf(')');
                json = json.Substring(lpos + 1, rpos - lpos - 1).Trim();
            }
            JsonObject me = JsonValue.LoadJson(json) as JsonObject;
            if (me == null || me.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            dict.Add("openid", (me["openid"] as JsonString).Value);
            dict.Add("oauth_consumer_key", _options.ClientId);
            url = "https://graph.qq.com/user/get_user_info?" + HttpBuildQuery(dict);
            json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2Member()
            {
                Type = Key,
                UserId = me["openid"] as JsonString,
                ScreenName = user["nickname"] as JsonString,
                UserName = "",
                Location = "",
                Description = "",
                Image = user["figureurl"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
