using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Cnaws.Json;
using Cnaws.Templates;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2
{
    public enum OAuth2MethodType
    {
        GET,
        POST
    }

    public abstract class OAuth2Provider
    {
        protected OAuth2ProviderOptions _options;
        private SortedDictionary<string, object> _parameters;
        protected List<string> _scope;
        private string _redirectUri;

        protected OAuth2Provider(OAuth2ProviderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (string.IsNullOrEmpty(options.ClientId))
                throw new ArgumentException("Required option not provided: ClientId");
            _options = options;
            _parameters = new SortedDictionary<string, object>();
            if (options.Scope.Count > 0)
                _scope = options.Scope;
            else
                _scope = new List<string>();
            _redirectUri = HttpContext.Current.Request.Url.ToString();
        }

        public static OAuth2Provider Create(OAuth2ProviderOptions options, string name)
        {
            try
            {
                Type type = Type.GetType(string.Concat("Cnaws.Passport.OAuth2.Providers.", name, ",Cnaws.Passport"), true, true);
                object result = Activator.CreateInstance(type, new object[] { options });
                if (TType<OAuth2Provider>.Type.IsAssignableFrom(result.GetType()))
                    return (OAuth2Provider)result;
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// provider name
        /// </summary>
        public string Key
        {
            get { return GetType().Name; }
        }
        /// <summary>
        /// provider human name
        /// </summary>
        public abstract string Name { get; }
        public virtual Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }
        /// <summary>
        /// state key name, for some unfollowing spec kids
        /// </summary>
        public virtual string StateKey
        {
            get { return "state"; }
        }
        /// <summary>
        /// error key name, for some unfollowing spec kids
        /// </summary>
        public virtual string ErrorKey
        {
            get { return "error"; }
        }
        /// <summary>
        /// client_id key name, for some unfollowing spec kids
        /// </summary>
        public virtual string ClientIdKey
        {
            get { return "client_id"; }
        }
        /// <summary>
        /// client_secret key name, for some unfollowing spec kids
        /// </summary>
        public virtual string ClientSecretKey
        {
            get { return "client_secret"; }
        }
        /// <summary>
        /// redirect_uri key name, for some unfollowing spec kids
        /// </summary>
        public virtual string RedirectUriKey
        {
            get { return "redirect_uri"; }
        }
        /// <summary>
        /// access_token key name, for some unfollowing spec kids
        /// </summary>
        public virtual string AccessTokenKey
        {
            get { return "access_token"; }
        }
        /// <summary>
        /// uid key name name, for some unfollowing spec kids
        /// </summary>
        public virtual string UidKey
        {
            get { return "uid"; }
        }
        public virtual bool ApprovalPrompt
        {
            get { return true; }
        }
        /// <summary>
        /// additional request parameters to be used for remote requests
        /// </summary>
        public virtual string Callback
        {
            get { return null; }
        }
        ///// <summary>
        ///// additional request parameters to be used for remote requests
        ///// </summary>
        //protected List<OAuth2Parameter> Parameters
        //{
        //    get { return _parameters; }
        //}
        /// <summary>
        /// the method to use when requesting tokens
        /// </summary>
        protected virtual OAuth2MethodType Method
        {
            get { return OAuth2MethodType.GET; }
        }
        /// <summary>
        /// default scope (useful if a scope is required for user info)
        /// </summary>
        protected List<string> Scope
        {
            get { return _scope; }
        }
        /// <summary>
        /// scope separator, most use "," but some like Google are spaces
        /// </summary>
        protected virtual string ScopeSeperator
        {
            get { return ","; }
        }
        /// <summary>
        /// Returns the authorization URL for the provider.
        /// </summary>
        public abstract string UrlAuthorize { get; }
        /// <summary>
        /// Returns the access token endpoint for the provider.
        /// </summary>
        public abstract string UrlAccessToken { get; }

        public abstract OAuth2Member GetUserInfo(OAuth2TokenAccess token);

        public OAuth2Provider AddParameter(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                _parameters.Remove(name);
            else
                _parameters[name] = value;
            return this;
        }

        /*
        * Get an authorization code from Provider Service.  Redirects to Provider Authorization Page, which this redirects back to the app using the redirect address you've set.
        */
        public void Authorize()
        {
            HttpResponse response = HttpContext.Current.Response;
            string state = Guid.NewGuid().ToString("N").ToLower();
            response.Cookies["OAuth2_State"].Value = state;
            AddParameter(ClientIdKey, _options.ClientId);
            AddParameter(RedirectUriKey, string.IsNullOrEmpty(_options.RedirectUri) ? _redirectUri : _options.RedirectUri);
            AddParameter(StateKey, state);
            if (_scope.Count > 0)
            {
                if (_scope.Count > 1)
                    AddParameter("scope", string.Join(ScopeSeperator, _scope.ToArray()));
                else
                    AddParameter("scope", _scope[0]);
            }
            AddParameter("response_type", "code");
            if (ApprovalPrompt)
                AddParameter("approval_prompt", "force");
            response.ContentType = "text/html";
            response.Write(@"<!doctype html>
<html>
<head>
<title>登录跳转</title>
<meta http-equiv=""refresh"" content=""0; url=");
            response.Write(UrlAuthorize);
            response.Write('?');
            response.Write(HttpBuildQuery(_parameters));
            response.Write(@""" />
</head>
<body style=""font-size:14px;padding:10px;text-align:center;"">
如果您的浏览器没有跳转，请<a href=""");
            response.Write(UrlAuthorize);
            response.Write('?');
            response.Write(HttpBuildQuery(_parameters));
            response.Write(@""">点击这里</a>。
</body>
</html>");
            response.End();
            //try { HttpContext.Current.Response.Redirect(UrlAuthorize + "?" + OAuthUtil.BuildQuery(_parameters)); }
            //catch { }
        }
        /*
        * Get access to the API
        *
        * @param	string	The access code
        * @return	object	Success or failure along with the response details
        */
        public OAuth2Token Access()
        {
            string code = HttpContext.Current.Request.QueryString["code"];
            string state = HttpContext.Current.Request[StateKey];
            HttpCookie cookie = HttpContext.Current.Request.Cookies["OAuth2_State"];
            if (cookie == null || state != cookie.Value)
                throw new OAuth2Exception(403, "The state does not match. Maybe you are a victim of CSRF.");

            AddParameter(ClientIdKey, _options.ClientId);
            AddParameter(ClientSecretKey, _options.ClientSecret);
            AddParameter("grant_type", _options.GrantType);
            switch ((OAuth2GrantType)_parameters["grant_type"])
            {
                case OAuth2GrantType.authorization_code:
                    AddParameter("code", code);
                    AddParameter(RedirectUriKey, string.IsNullOrEmpty(_options.RedirectUri) ? _redirectUri : _options.RedirectUri);
                    break;
                case OAuth2GrantType.refresh_token:
                    AddParameter("refresh_token", code);
                    break;
                default:
                    throw new IndexOutOfRangeException("GrantType value \"" + _parameters["grant_type"].ToString() + "\" must be either authorization_code or refresh_token");
            }

            string response = null;
            string url = UrlAccessToken;
            switch (Method)
            {
                case OAuth2MethodType.GET:
                    url += "?" + HttpBuildQuery(_parameters);
                    response = HttpGetContents(url);
                    break;
                case OAuth2MethodType.POST:
                    response = HttpGetContents(url, HttpBuildQuery(_parameters));
                    break;
                default:
                    throw new IndexOutOfRangeException("Method value \"" + Method + "\" must be either GET or POST");
            }

            JsonObject result = ParseResponse(response);
            if (result == null || result.ContainsKey(ErrorKey) || !result.ContainsKey("access_token"))
                throw new OAuth2Exception(500, response);

            result["uid_key"] = new JsonString(UidKey);
            result["access_token_key"] = new JsonString(AccessTokenKey);

            switch ((OAuth2GrantType)_parameters["grant_type"])
            {
                case OAuth2GrantType.authorization_code: return new OAuth2TokenAccess(result);
                    //case OAuth2GrantType.refresh_token:
            }
            throw new IndexOutOfRangeException("GrantType value \"" + _parameters["grant_type"].ToString() + "\" must be either authorization_code or refresh_token");
        }
        protected JsonObject ParseResponse(string response)
        {
            JsonObject result = null;
            if (response.IndexOf("callback") > -1)
            {
                int lpos = response.IndexOf('(');
                int rpos = response.LastIndexOf(')');
                response = response.Substring(lpos + 1, rpos - lpos - 1).Trim();
                result = JsonValue.LoadJson(response) as JsonObject;
            }
            else if (response.IndexOf('&') > -1)
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(response);
                result = new JsonObject();
                foreach (string key in nvc.AllKeys)
                {
                    if (string.Equals(key, "expires", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "expires_in", StringComparison.OrdinalIgnoreCase))
                        result[key] = new JsonNumber(int.Parse(nvc[key]));
                    else
                        result[key] = new JsonString(nvc[key]);
                }
            }
            else
            {
                result = JsonValue.LoadJson(response) as JsonObject;
            }
            return result;
        }

        protected static string HttpBuildQuery(SortedDictionary<string, object> dict)
        {
            if (dict != null)
            {
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string key in dict.Keys)
                {
                    if (i++ > 0) sb.Append('&');
                    sb.Append(key);
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode((string)Convert.ChangeType(dict[key], TType<string>.Type)));
                }
                return sb.ToString();
            }
            return string.Empty;
        }
        protected static string HttpGetContents(string url, byte[] data = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (data != null)
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream s = request.GetRequestStream())
                    s.Write(data, 0, data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader r = new StreamReader(s, Encoding.UTF8))
                {
                    return r.ReadToEnd();
                }
            }
        }
        protected static string HttpGetContents(string url, string data)
        {
            return HttpGetContents(url, ((data != null) ? Encoding.UTF8.GetBytes(data) : null));
        }
    }
}
