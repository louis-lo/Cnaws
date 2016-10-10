using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class Baidu : OAuth2Provider
    {
        public Baidu(OAuth2ProviderOptions options)
            : base(options)
        {
        }
        
        public override string Name
        {
            get { return "百度"; }
        }
        public override string UidKey
        {
            get { return "uid"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://openapi.baidu.com/oauth/2.0/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://openapi.baidu.com/oauth/2.0/token"; }
        }

        public override OAuth2Member GetUserInfo(OAuth2TokenAccess token)
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            dict.Add("access_token", token.AccessToken);
            dict.Add("uid", token.UserId);
            string url = "https://openapi.baidu.com/rest/2.0/passport/users/getLoggedInUser?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2Member()
            {
                Type = Key,
                UserId = user["uid"] as JsonString,
                ScreenName = user["uname"] as JsonString,
                UserName = "",
                Location = "",
                Description = "",
                Image = "http://tb.himg.baidu.com/sys/portraitn/item/" + (user["portrait"] as JsonString).Value,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
