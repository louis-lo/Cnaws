using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class _163Provider : OAuth2Provider
    {
        public _163Provider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType._163; }
        }
        public override string Name
        {
            get { return "网易微博"; }
        }
        public override string UidKey
        {
            get { return "user_id"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://api.t.163.com/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://api.t.163.com/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(2);
            dict.Add("access_token", token.AccessToken);
            dict.Add("user_id", token.UserId);
            string url = "https://api.t.163.com/users/show.json?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType._163,
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
