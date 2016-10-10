using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class DoubanProvider : OAuth2Provider
    {
        public DoubanProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.douban; }
        }
        public override string Name
        {
            get { return "豆瓣"; }
        }
        public override string UidKey
        {
            get { return "douban_user_id"; }
        }
        public override string ErrorKey
        {
            get { return "msg"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://www.douban.com/service/auth2/auth"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://www.douban.com/service/auth2/token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(1);
            dict.Add("access_token", token.AccessToken);
            string url = "https://api.douban.com/v2/user/" + token.UserId + "?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("msg"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.douban,
                UserId = user["id"] as JsonString,
                ScreenName = user["uid"] as JsonString,
                UserName = user["name"] as JsonString,
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
