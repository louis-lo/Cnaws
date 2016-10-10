using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class Taobao : OAuth2Provider
    {
        public Taobao(OAuth2ProviderOptions options)
            : base(options)
        {
        }
        
        public override string Name
        {
            get { return "淘宝"; }
        }
        public override string UidKey
        {
            get { return "taobao_user_id"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://oauth.taobao.com/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://oauth.taobao.com/token"; }
        }

        public override OAuth2Member GetUserInfo(OAuth2TokenAccess token)
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            dict.Add("access_token", token.AccessToken);
            dict.Add("method", "taobao.user.get");
            dict.Add("v", "2.0");
            dict.Add("format", "json");
            dict.Add("fields", "user_id,uid,nick,location,avatar");
            string url = "https://eco.taobao.com/router/rest?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error_response"))
                throw new OAuth2Exception(500, json);
            JsonObject data = user["user_get_response"] as JsonObject;
            if (data == null)
                throw new OAuth2Exception(500, json);
            JsonObject u = data["user"] as JsonObject;
            if (u == null)
                throw new OAuth2Exception(500, json);
            return new OAuth2Member()
            {
                Type = Key,
                UserId = u["user_id"] as JsonString,
                ScreenName = u["nick"] as JsonString,
                UserName = u["uid"] as JsonString,
                Location = u["location"] as JsonString,
                Description = "",
                Image = u["avatar"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
