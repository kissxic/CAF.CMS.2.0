//Contributor:  Nicholas Mayne

using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using CAF.Infrastructure.Core.Utilities.Randomizer;
using Newtonsoft.Json;
using CAF.WebSite.XLAuth.Models;
using System.Collections.Specialized;
using System.Net.Cache;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CAF.WebSite.XLAuth.Core
{
    public class XLProviderAuthorizer : IOAuthProviderXLAuthorizer
    {
        #region Fields
        //meta正则表达式
        private static Regex _metaregex = new Regex("<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICommonServices _services;
        #endregion

        #region Ctor

        public XLProviderAuthorizer(IExternalAuthorizer authorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            HttpContextBase httpContext,
            ICommonServices services)
        {
            this._authorizer = authorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._httpContext = httpContext;
            this._services = services;
        }

        #endregion

        #region Utilities


        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            var settings = _services.Settings.LoadSetting<XLExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            //返回的随机值
            string backSalt = _services.WebHelper.QueryString<string>("state");
            //Authorization Code
            string code = _services.WebHelper.QueryString<string>("code");
            //保存在session中随机值
            string salt = _services.Cache.Get<string>("xlAuthLoginSalt");

            var authUrl = GenerateLocalCallbackUri().AbsoluteUri;
            if (backSalt.Length > 0 && code.Length > 0 && salt.Length > 0 && backSalt == salt)
            {
                var client = GetOpenAuthClient();

                client.GetAccessTokenByCode(code);
                string accessToken = null;
                string uid = null;
                if (client.IsAuthorized)
                {
                    //用session记录access token
                    accessToken = client.AccessToken;
                    uid = client.UID;
                }
                else
                {
                    throw new Exception("Authentication result does not contain accesstoken and uid data");
                }

                if (!accessToken.IsEmail())
                {

                    // 调用获取获取用户信息api
                    // 参考：http://open.weibo.com/wiki/2/users/show
                    var response = client.HttpGet("users/show.json", new
                    {
                        uid = client.UID
                    });

                    if (response.IsSuccessStatusCode)
                    {

                        OAuthUser oAuthUser = JsonConvert.DeserializeObject<OAuthUser>(response.Content.ReadAsStringAsync().Result);
                        //将返回结果序列化为对象
                        var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                        {
                            ExternalIdentifier = uid,
                            OAuthToken = accessToken,
                            OAuthAccessToken = uid,
                        };
                        if (_externalAuthenticationSettings.AutoRegisterEnabled)
                            ParseClaims(oAuthUser, parameters);
                        var result = _authorizer.Authorize(parameters);

                        return new AuthorizeState(returnUrl, result);
                    }
                    else
                    {

                    }

                }

            }
            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            var error = "Unknown error";
            state.AddError(error);
            return state;
        }

        private void ParseClaims(OAuthUser user, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();
            claims.Contact.UserName = user.screen_name;

            string name = user.screen_name;
            claims.Name = new NameClaims();
            if (!name.IsEmpty())
            {
                var nameSplit = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameSplit.Length >= 2)
                {
                    claims.Name.First = nameSplit[0];
                    claims.Name.Last = nameSplit[1];
                }
                else
                {
                    claims.Name.Last = nameSplit[0];
                }

            }
            parameters.AddClaim(claims);
        }

        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var authUrl = GenerateServiceLoginUrl().AbsoluteUri;
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }
        /// <summary>
        /// 回调地址
        /// </summary>
        /// <returns></returns>
        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}Plugins/CAF.WebSite.XLAuth/logincallback/", _services.WebHelper.GetSiteLocation());
            return new Uri(url);
        }
        /// <summary>
        /// 登录授权地址
        /// </summary>
        /// <returns></returns>
        private Uri GenerateServiceLoginUrl()
        {
            var settings = _services.Settings.LoadSetting<XLExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            var salt = _services.Cache.Get<string>("xlAuthLoginSalt", () =>
             {
                 return SomeRandom.Integer(100000000, 999999999).ToString();
             }, TimeSpan.FromMinutes(60));
            var client = GetOpenAuthClient(salt);

            return new Uri(client.GetAuthorizationUrl());
        }

        /// <summary>
        /// 封装一个方法来初始化OpenAuth客户端
        /// </summary>
        /// <returns></returns>
        private SinaWeiboClient GetOpenAuthClient(string state = null, string accessToken = null, string uid = null)
        {
            var settings = _services.Settings.LoadSetting<XLExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            var client = new SinaWeiboClient(settings.ClientKeyIdentifier ?? string.Empty, settings.ClientSecret ?? string.Empty, GenerateLocalCallbackUri().AbsoluteUri, state, accessToken, uid);

            return client;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Authorize response
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="verifyResponse">true - Verify response;false - request authentication;null - determine automatically</param>
        /// <returns>Authorize state</returns>
        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (!verifyResponse.HasValue)
                throw new ArgumentException("Facebook plugin cannot automatically determine verifyResponse property");

            if (verifyResponse.Value)
            {
                return VerifyAuthentication(returnUrl);
            }
            else
            {
                return RequestAuthentication(returnUrl);
            }
        }

        #endregion
    }
}