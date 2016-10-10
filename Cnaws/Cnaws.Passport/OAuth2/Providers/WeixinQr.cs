using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class Weixinqr : OAuth2Provider
    {
        private const string API_URL = "https://api.weixin.qq.com/";

        public Weixinqr(OAuth2ProviderOptions options)
            : base(options)
        {
            base.Scope.Add("snsapi_login");
        }
        
        public override string Name
        {
            get { return "微信"; }
        }
        public override string UidKey
        {
            get { return "openid"; }
        }
        public override string ClientIdKey
        {
            get { return "appid"; }
        }
        public override string ClientSecretKey
        {
            get { return "secret"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "https://open.weixin.qq.com/connect/qrconnect"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://api.weixin.qq.com/sns/oauth2/access_token"; }
        }

        public void ScopeMin()
        {
            base.Scope.Clear();
            base.Scope.Add("snsapi_base");
        }
        public override OAuth2Member GetUserInfo(OAuth2TokenAccess token)
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            dict.Add("access_token", token.AccessToken);
            dict.Add("openid", token.UserId);
            dict.Add("lang", "zh_CN");
            string url = API_URL + "sns/userinfo?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("errcode"))
                throw new OAuth2Exception(500, json);
            return new OAuth2Member()
            {
                Type = Key,
                UserId = user["openid"] as JsonString,
                ScreenName = user["nickname"] as JsonString,
                UserName = user["nickname"] as JsonString,
                Location = user["province"] as JsonString,
                Description = "",
                Image = user["headimgurl"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
