using System;
using Cnaws.Json;

namespace Cnaws.Passport.OAuth2
{
    public abstract class OAuth2Token
    {
    }

    public class OAuth2TokenAuthorize : OAuth2Token
    {
        protected internal string Code;
        protected internal string RedirectUri;

        public OAuth2TokenAuthorize(JsonObject options)
        {
            if (!options.ContainsKey("code"))
                throw new ArgumentException("Required option not passed: code");
            if (!options.ContainsKey("redirect_uri"))
                throw new ArgumentException("Required option not passed: redirect_uri");
            Code = options["code"] as JsonString;
            RedirectUri = options["redirect_uri"] as JsonString;
        }

        public override string ToString()
        {
            return Code;
        }
    }

    public class OAuth2TokenAccess : OAuth2Token
    {
        protected internal string AccessToken;
        protected internal int Expires;
        protected internal string RefreshToken;
        protected internal string UserId;

        /**
         * Sets the token, expiry, etc values.
         *
         * @param   array  $options   token options
         *
         * @throws Exception if required options are missing
         */
        public OAuth2TokenAccess(JsonObject options)
        {
            JsonString value = options["access_token_key"] as JsonString;
            if (value == null)
                throw new ArgumentException("Required option not passed: access_token");
            value = options[value] as JsonString;
            if (value == null)
                throw new ArgumentException("Required option not passed: access_token");
            AccessToken = value.Value;
            value = options["uid_key"] as JsonString;
            if (value != null)
            {
                value = options[value] as JsonString;
                if (value != null)
                    UserId = value.Value;
            }
            value = options["x_mailru_vid"] as JsonString;
            if (value != null)
                UserId = value.Value;
            JsonNumber ivalue = options["expires_in"] as JsonNumber;
            if (ivalue != null)
                Expires = (int)ivalue.Value;
            ivalue = options["expires"] as JsonNumber;
            if (ivalue != null)
                Expires = (int)ivalue.Value;
            value = options["refresh_token"] as JsonString;
            if (value != null)
                RefreshToken = value.Value;
        }

        /// <summary>
        /// Returns the token key.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AccessToken;
        }
    }
}
