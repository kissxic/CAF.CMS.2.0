using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CAF.WebSite.WeiXinAuth.Core
{
    public class WeiXinClient : OpenAuthenticationClientBase
    {
        //微信登录授权
        const string AUTH_URL = "https://open.weixin.qq.com/connect/qrconnect";
        //微信OpenId授权
        const string TOKEN_URL = "https://api.weixin.qq.com/sns/oauth2/access_token";
        //微信用户信息授权
        const string API_URL = "https://api.weixin.qq.com/";
       
        public string OpenId
        {
            get;
            set;
        }

        public WeiXinClient(string appKey, string appSecret, string callbackUrl, string state = null, string accessToken = null, string openId = null)
                : base(appKey, appSecret, callbackUrl, state, accessToken)
        {
            ClientName = "WeiXin";
            OpenId = openId;

            if (!(string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(openId)))
            {
                isAccessTokenSet = true;
            }
        }

        protected override string AuthorizationCodeUrl
        {
            get { return AUTH_URL; }
        }

        protected override string AccessTokenUrl
        {
            get { return TOKEN_URL; }
        }

        protected override string BaseApiUrl
        {
            get { return API_URL; }
        }
        /// <summary>
        ///    //微信登录授权 获取Code Appid是微信应用id
        /// </summary>
        /// <returns></returns>
        public override string GetAuthorizationUrl()
        {
            var ub = new UriBuilder(AuthorizationCodeUrl);
            ub.Query = string.Format("appid={0}&redirect_uri={2}&response_type=code&scope=snsapi_login&state={1}#wechat_redirect", ClientId, State, Uri.EscapeDataString(CallbackUrl));
            return ub.ToString();
        }
     
        /// <summary>
        /// 用Code换取Openid
        /// </summary>
        /// <param name="code"></param>
        public override void GetAccessTokenByCode(string code)
        {
 
            var response = HttpPost(TOKEN_URL, new
            {
                appid = ClientId,
                secret = ClientSecret,
                grant_type = "authorization_code",
                code = code,
            });

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

            var result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if (result["access_token"] == null)
            {
                return;
            }
            AccessToken = result.Value<string>("access_token");
            OpenId= result.Value<string>("openid");
            isAccessTokenSet = true;
        }

        public override Task<HttpResponseMessage> HttpGetAsync(string api, Dictionary<string, object> parameters)
        {
            if (IsAuthorized)
            {
                if (parameters == null)
                    parameters = new Dictionary<string, object>();

                if (!parameters.ContainsKey("source"))
                {
                    parameters["source"] = ClientId;
                }

                if (!parameters.ContainsKey("access_token"))
                {
                    parameters["access_token"] = AccessToken;
                }
            }



            return base.HttpGetAsync(api, parameters);
        }


        public override Task<HttpResponseMessage> HttpPostAsync(string api, Dictionary<string, object> parameters)
        {
            if (IsAuthorized)
            {
                if (parameters == null)
                    parameters = new Dictionary<string, object>();

                if (!parameters.ContainsKey("source"))
                {
                    parameters["source"] = ClientId;
                }

                if (!parameters.ContainsKey("access_token"))
                {
                    parameters["access_token"] = AccessToken;
                }
            }

            return base.HttpPostAsync(api, parameters);
        }


    }
}