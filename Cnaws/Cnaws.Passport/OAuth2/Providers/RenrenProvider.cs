using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class RenrenProvider : OAuth2Provider
    {
        public RenrenProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.renren; }
        }
        public override string Name
        {
            get { return "人人"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://graph.renren.com/oauth/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://graph.renren.com/oauth/token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(5);
            dict.Add("access_token", token.AccessToken);
            dict.Add("format", "JSON");
            dict.Add("v", "1.0");
            dict.Add("call_id", Guid.NewGuid().ToString("N"));
            dict.Add("method", "users.getInfo");
            string url = "https://api.renren.com/restserver.do";
            string json = HttpGetContents(url, HttpBuildQuery(dict));
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            //! is_array($user) OR ! isset($user[0]) OR ! ($user = $user[0]) OR array_key_exists("error_code", $user)
            if (user == null || user.ContainsKey("error_code"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.renren,
                UserId = user["uid"] as JsonString,
                ScreenName = user["name"] as JsonString,
                UserName = "",
                Location = "",
                Description = "",
                Image = user["tinyurl"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
