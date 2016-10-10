using System;
using System.Collections.Generic;
using Cnaws.Json;
using Cnaws.Passport.Modules;

namespace Cnaws.Passport.OAuth2.Providers
{
    internal sealed class _10086Provider : OAuth2Provider
    {
        public _10086Provider(OAuth2ProviderOptions options)
            : base(options)
        {
        }

        public override OAuth2ProviderType Type
        {
            get { return OAuth2ProviderType._10086; }
        }
        public override string Name
        {
            get { return "移动微博"; }
        }
        public override string ClientIdKey
        {
            get { return "api_key"; }
        }
        public override string ClientSecretKey
        {
            get { return "api_secret"; }
        }
        public override string ErrorKey
        {
            get { return "error_code"; }
        }
        public override string AccessTokenKey
        {
            get { return "session_key"; }
        }
        protected override OAuth2MethodType Method
        {
            get { return OAuth2MethodType.POST; }
        }
        public override string UrlAuthorize
        {
            get { return "http://oapi.weibo.10086.cn/oauth/authorize.php"; }
        }
        public override string UrlAccessToken
        {
            get
            {
                //oauth1.0强行转换成2.0的后果
                AddParameter("namespace", "mig");
                AddParameter("random", "");
                AddParameter("mig", "");
                return "http://oapi.weibo.10086.cn/oauth/token.php";
                //$this->params['namespace'] = 'mig';
                //$this->params['random'] = substr(md5(time()), 0, 16);
                //$this->params['mig'] = md5($_GET['code'].substr($this->params['random'], -1, 12).$this->client_secret.$this->params['namespace']);
                //return 'http:// oapi.weibo.10086.cn/oauth/token.php';
            }
        }

        public override OAuth2UserInfo GetUserInfo(OAuth2TokenAccess token)
        {
            throw new NotImplementedException();
        }
    //public function get_user_info(OAuth2_Token_Access $token)
    //{  	
    //    $call_id = time();
    //    $mi_sig = md5('api_key'.$this->client_id.'call_id'.$call_id.'session_key'.$token->access_token.'v2.0'.$this->client_secret);

    //    $url = 'http://oapi.weibo.10086.cn/users/getloggedinuser.json?'.http_build_query(array(
    //        'session_key' => $token->access_token,
    //        'api_key' => $this->client_id,
    //        'v' => '2.0',
    //        'call_id' => $call_id,
    //        'mi_sig' => $mi_sig
    //    ));
    //    $user = json_decode(file_get_contents($url));
                
    //    if ($user->error_code > 0)
    //    {
    //        throw new OAuth2_Exception((array) $user);
    //    }

    //    $uid = $user->uid;

    //    $url = 'http://oapi.weibo.10086.cn/users/getinfo.json?'.http_build_query(array(
    //        'session_key' => $token->access_token,
    //        'api_key' => $this->client_id,
    //        'v' => '2.0',
    //        'call_id' => $call_id,
    //        'mi_sig' => $mi_sig
    //    ));
    //    $user = json_decode(file_get_contents($url));

    //    if ($user->error_code > 0)
    //    {
    //        throw new OAuth2_Exception((array) $user);
    //    }
                
    //    // Create a response from the request
    //    return array(
    //        'via' => '10086',
    //        'uid' => $uid,
    //        'screen_name' => $user->screen_name,
    //        'name' => $user->username,
    //        'location' => '',
    //        'description' => '',
    //        'image' => $user->tinyurl,
    //        'access_token' => $token->access_token,
    //        'expire_at' => $token->expires,
    //        'refresh_token' => $token->refresh_token
    //    );
    }
}
