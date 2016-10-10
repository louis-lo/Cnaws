using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class TianyiProvider : OAuth2Provider
    {
        public TianyiProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.tianyi; }
        }
        public override string Name
        {
            get { return "天翼"; }
        }
        public override string UidKey
        {
            get { return "p_user_id"; }
        }
        public override string ClientIdKey
        {
            get { return "app_id"; }
        }
        public override string ClientSecretKey
        {
            get { return "app_secret"; }
        }
        public override string ErrorKey
        {
            get { return "res_code"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://oauth.api.189.cn/emp/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://oauth.api.189.cn/emp/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(2);
            dict.Add("access_token", token.AccessToken);
            dict.Add("app_id", _options.ClientId);
            string url = "http://api.189.cn/upc/vitual_identity/user_network_info?type=json&" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.tianyi,
                UserId = token.UserId,
                ScreenName = user["user_nickname"] as JsonString,
                UserName = "",
                Location = "",
                Description = user["user_selfdesc"] as JsonString,
                Image = "",
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
