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
using CAF.WebSite.WeiXinAuth.Models;
using System.Collections.Specialized;
using System.Net.Cache;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CAF.WebSite.WeiXinAuth.Core
{
    public class WXProviderAuthorizer : IOAuthProviderWXAuthorizer
    {
        #region Fields
        //meta������ʽ
        private static Regex _metaregex = new Regex("<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICommonServices _services;
        #endregion

        #region Ctor

        public WXProviderAuthorizer(IExternalAuthorizer authorizer,
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
            var settings = _services.Settings.LoadSetting<WXExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            //���ص����ֵ
            string backSalt = _services.WebHelper.QueryString<string>("state");
            //Authorization Code
            string code = _services.WebHelper.QueryString<string>("code");
            //������session�����ֵ
            string salt = _services.Cache.Get<string>("xlAuthLoginSalt");

            var authUrl = GenerateLocalCallbackUri().AbsoluteUri;
            if (backSalt.Length > 0 && code.Length > 0 && salt.Length > 0 && backSalt == salt)
            {
                var client = GetOpenAuthClient();

                client.GetAccessTokenByCode(code);
                string accessToken = null;
                string openId = null;
                if (client.IsAuthorized)
                {
                    //��session��¼access token
                    accessToken = client.AccessToken;
                    openId = client.OpenId;
                }
                else
                {
                    throw new Exception("Authentication result does not contain accesstoken and uid data");
                }

                if (!accessToken.IsEmail())
                {


                    // ���û�ȡ��ȡ�û���Ϣapi
                    // �ο���https://api.weixin.qq.com/sns/userinfo
                    var response = client.HttpGet("sns/userinfo", new
                    {
                        access_token = accessToken,
                        openid = openId
                    });

                    if (response.IsSuccessStatusCode)
                    {

                        var userResult = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        if (userResult["errcode"] == null)
                        {
                            WeiXinUserInfo oAuthUser = JsonConvert.DeserializeObject<WeiXinUserInfo>(response.Content.ReadAsStringAsync().Result);
                            //�����ؽ�����л�Ϊ����
                            var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                            {
                                ExternalIdentifier = openId,
                                OAuthToken = accessToken,
                                OAuthAccessToken = openId,
                            };
                            if (_externalAuthenticationSettings.AutoRegisterEnabled)
                                ParseClaims(oAuthUser, parameters);
                            var result = _authorizer.Authorize(parameters);

                            return new AuthorizeState(returnUrl, result);
                        }
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

        private void ParseClaims(WeiXinUserInfo user, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();
            claims.Contact.UserName = user.nickname;

            string name = user.nickname;
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
        /// �ص���ַ
        /// </summary>
        /// <returns></returns>
        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}Plugins/CAF.WebSite.WeiXinAuth/logincallback/", _services.WebHelper.GetSiteLocation());
            return new Uri(url);
        }
        /// <summary>
        /// ��¼��Ȩ��ַ
        /// </summary>
        /// <returns></returns>
        private Uri GenerateServiceLoginUrl()
        {
            var settings = _services.Settings.LoadSetting<WXExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            var salt = _services.Cache.Get<string>("xlAuthLoginSalt", () =>
             {
                 return SomeRandom.Integer(100000000, 999999999).ToString();
             }, TimeSpan.FromMinutes(60));
            var client = GetOpenAuthClient(salt);

            return new Uri(client.GetAuthorizationUrl());
        }

        /// <summary>
        /// ��װһ����������ʼ��OpenAuth�ͻ���
        /// </summary>
        /// <returns></returns>
        private WeiXinClient GetOpenAuthClient(string state = null, string accessToken = null, string uid = null)
        {
            var settings = _services.Settings.LoadSetting<WXExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            var client = new WeiXinClient(settings.ClientKeyIdentifier ?? string.Empty, settings.ClientSecret ?? string.Empty, GenerateLocalCallbackUri().AbsoluteUri, state, accessToken, uid);

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