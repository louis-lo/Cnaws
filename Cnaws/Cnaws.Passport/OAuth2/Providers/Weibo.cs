using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class Weibo : OAuth2Provider
    {
        public Weibo(OAuth2ProviderOptions options)
            : base(options)
        {
        }
        
        public override string Name
        {
            get { return "新浪微博"; }
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
            get { return "https://api.weibo.com/oauth2/authorize"; }
        }
        public override string UrlAccessToken
        {
            get { return "https://api.weibo.com/oauth2/access_token"; }
        }

        public override OAuth2Member GetUserInfo(OAuth2TokenAccess token)
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            dict.Add("access_token", token.AccessToken);
            dict.Add("uid", token.UserId);
            string url = "https://api.weibo.com/2/users/show.json?" + HttpBuildQuery(dict);
            string json = HttpGetContents(url);
            JsonObject user = JsonValue.LoadJson(json) as JsonObject;
            if (user == null || user.ContainsKey("error"))
                throw new OAuth2Exception(500, json);
            return new OAuth2Member()
            {
                Type = Key,
                UserId = user["id"] as JsonString,
                ScreenName = user["screen_name"] as JsonString,
                UserName = user["name"] as JsonString,
                Location = user["location"] as JsonString,
                Description = user["description"] as JsonString,
                Image = user["profile_image_url"] as JsonString,
                AccessToken = token.AccessToken,
                ExpireAt = token.Expires,
                RefreshToken = token.RefreshToken
            };
        }
    }
}
