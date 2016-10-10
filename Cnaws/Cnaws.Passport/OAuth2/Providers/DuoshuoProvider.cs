using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class DuoshuoProvider : OAuth2Provider
    {
        public DuoshuoProvider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType.duoshuo; }
        }
        public override string Name
        {
            get { return "多说"; }
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
            get { throw new OAuth2Exception(403, "亲，多说的授权不是从这里进来的."); }
        }
        public override string UrlAccessToken
        {
            get { return "http://api.duoshuo.com/oauth2/access_token"; }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            return new OAuth2UserInfo()
            {
                Type = OAuth2ProviderType.duoshuo,
                UserId = token.UserId,
                ScreenName = token.UserId,
                UserName = "",
                Location = "",
                Description = "",
                Image = "",
                AccessToken = token.AccessToken,
                ExpireAt = 0,
                RefreshToken = ""
            };
        }
    }
}
