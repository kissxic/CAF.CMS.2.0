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
using CAF.WebSite.QQAuth.Models;
using System.Collections.Specialized;
using System.Net.Cache;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CAF.WebSite.QQAuth.Core
{
    public class QQProviderAuthorizer : IOAuthProviderQQAuthorizer
    {
        #region Fields
        //meta正则表达式
        private static Regex _metaregex = new Regex("<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICommonServices _services;
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";
        private const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";
        private const string UserInfoEndpoint = "https://graph.qq.com/user/get_user_info";
        private const string OpenIDEndpoint = "https://graph.qq.com/oauth2.0/me";

        #endregion

        #region Ctor

        public QQProviderAuthorizer(IExternalAuthorizer authorizer,
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
            var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            //返回的随机值
            string backSalt = _services.WebHelper.QueryString<string>("state");
            //Authorization Code
            string code = _services.WebHelper.QueryString<string>("code");
            //保存在session中随机值
            string salt = _services.Cache.Get<string>("qqAuthLoginSalt");

            var authUrl = GenerateLocalCallbackUri().AbsoluteUri;
            if (backSalt.Length > 0 && code.Length > 0 && salt.Length > 0 && backSalt == salt)
            {

                //构建获取Access Token的参数
                string accessTokenUri = TokenEndpoint +
                    "?client_id=" + Uri.EscapeDataString(settings.ClientKeyIdentifier) +
                    "&client_secret=" + Uri.EscapeDataString(settings.ClientSecret) +
                    "&redirect_uri=" + Uri.EscapeDataString(string.IsNullOrEmpty(returnUrl) ? authUrl : returnUrl) +
                    "&code=" + code +
                    "&grant_type=authorization_code";
                //发送获得Access Token的请求
                string oauthTokenResponse = GetRequestData(accessTokenUri, "get", null);
                //将返回结果解析成参数列表
                var tokenDict = QueryStringToDict(oauthTokenResponse);
                string accessToken = null;
                //Access Token值
                if (tokenDict.ContainsKey("access_token"))
                {
                    accessToken = tokenDict["access_token"];
                }
                else
                {
                    throw new Exception("Authentication result does not contain accesstoken data");
                }

                if (!accessToken.IsEmail())
                {
                    //通过上一步获取的Access Token，构建获得对应用户身份的OpenID的url
                    string openIDUri = OpenIDEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken);
                    //发送获得OpenID的请求
                    string openIDString = GetRequestData(openIDUri, "get", null);
                    openIDString = ExtractOpenIDCallbackBody(openIDString);
                    JObject openIDInfo = JObject.Parse(openIDString);
                    //OpenID值
                    var clientId = openIDInfo["client_id"].Value<string>();
                    var openId = openIDInfo["openid"].Value<string>();
                    //获取用户信息的url
                    string userInfoUri = UserInfoEndpoint +
                        "?access_token=" + Uri.EscapeDataString(accessToken) +
                        "&oauth_consumer_key=" + Uri.EscapeDataString(clientId) +
                        "&openid=" + Uri.EscapeDataString(openId);
                    //发送获取用户信息的请求
                    string userInfoString = GetRequestData(userInfoUri, "get", null);
                    OAuthUser oAuthUser = JsonConvert.DeserializeObject<OAuthUser>(userInfoString);

                    //将返回结果序列化为对象
                    var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                    {
                        ExternalIdentifier = openId,
                        OAuthToken = accessToken,
                        OAuthAccessToken = openId,
                    };
                    // userInfo["nickname"].Value<string>()
                    if (_externalAuthenticationSettings.AutoRegisterEnabled)
                        ParseClaims(oAuthUser, parameters);

                    var result = _authorizer.Authorize(parameters);

                    return new AuthorizeState(returnUrl, result);
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
            claims.Contact.UserName = user.Nickname;

            string name = user.Nickname;
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

        private static string PropertyValueIfExists(string property, IDictionary<string, JToken> dictionary)
        {
            return dictionary.ContainsKey(property) ? dictionary[property].ToString() : null;
        }

        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var authUrl = GenerateServiceLoginUrl().AbsoluteUri;
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }

        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}Plugins/CAF.WebSite.QQAuth/logincallback/", _services.WebHelper.GetSiteLocation());
            return new Uri(url);
        }

        private Uri GenerateServiceLoginUrl()
        {
            var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(_services.SiteContext.CurrentSite.Id);
            var salt = _services.Cache.Get<string>("qqAuthLoginSalt", () =>
             {
                 return SomeRandom.Integer(100000000, 999999999).ToString();
             }, TimeSpan.FromMinutes(60));

            var builder = new UriBuilder(AuthorizationEndpoint);
            var args = new Dictionary<string, string>();
            args.Add("client_id", Uri.EscapeDataString(settings.ClientKeyIdentifier ?? string.Empty));
            args.Add("redirect_uri", Uri.EscapeDataString(GenerateLocalCallbackUri().AbsoluteUri));
            args.Add("scope", "get_user_info");
            args.Add("state", salt);
            args.Add("response_type", "code");



            AppendQueryArgs(builder, args);

            return builder.Uri;


        }

        private void AppendQueryArgs(UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if ((args != null) && (args.Count<KeyValuePair<string, string>>() > 0))
            {
                StringBuilder builder2 = new StringBuilder(50 + (args.Count<KeyValuePair<string, string>>() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    builder2.Append(builder.Query.Substring(1));
                    builder2.Append('&');
                }
                builder2.Append(CreateQueryString(args));
                builder.Query = builder2.ToString();
            }
        }

        private string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any<KeyValuePair<string, string>>())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(args.Count<KeyValuePair<string, string>>() * 10);
            foreach (KeyValuePair<string, string> pair in args)
            {
                builder.Append(pair.Key);
                builder.Append('=');
                builder.Append(pair.Value);
                builder.Append('&');
            }
            builder.Length--;
            return builder.ToString();
        }

        private string ExtractOpenIDCallbackBody(string callbackString)
        {
            int leftBracketIndex = callbackString.IndexOf('{');
            int rightBracketIndex = callbackString.IndexOf('}');
            if (leftBracketIndex >= 0 && rightBracketIndex >= 0)
            {
                return callbackString.Substring(leftBracketIndex, rightBracketIndex - leftBracketIndex + 1).Trim();
            }
            return callbackString;
        }




        private IDictionary<string, string> QueryStringToDict(string str)
        {
            var strArr = str.Split('&');
            var dict = new Dictionary<string, string>(strArr.Length);
            foreach (var s in strArr)
            {
                var equalSymbolIndex = s.IndexOf('=');
                if (equalSymbolIndex > 0 && equalSymbolIndex < s.Length - 1)
                {
                    dict.Add(
                        s.Substring(0, equalSymbolIndex),
                        s.Substring(equalSymbolIndex + 1, s.Length - equalSymbolIndex - 1));
                }
            }
            return dict;
        }

        /// <summary>
        /// 获得参数列表
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static NameValueCollection GetParmList(string data)
        {
            NameValueCollection parmList = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(data))
            {
                int length = data.Length;
                for (int i = 0; i < length; i++)
                {
                    int startIndex = i;
                    int endIndex = -1;
                    while (i < length)
                    {
                        char c = data[i];
                        if (c == '=')
                        {
                            if (endIndex < 0)
                                endIndex = i;
                        }
                        else if (c == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key;
                    string value;
                    if (endIndex >= 0)
                    {
                        key = data.Substring(startIndex, endIndex - startIndex);
                        value = data.Substring(endIndex + 1, (i - endIndex) - 1);
                    }
                    else
                    {
                        key = data.Substring(startIndex, i - startIndex);
                        value = string.Empty;
                    }
                    parmList[key] = value;
                    if ((i == (length - 1)) && (data[i] == '&'))
                        parmList[key] = string.Empty;
                }
            }
            return parmList;
        }

        /// <summary>
        /// 获得http请求数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">发送数据</param>
        /// <returns></returns>
        public static string GetRequestData(string url, string postData)
        {
            return GetRequestData(url, "post", postData);
        }

        /// <summary>
        /// 获得http请求数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postData">发送数据</param>
        /// <returns></returns>
        public static string GetRequestData(string url, string method, string postData)
        {
            return GetRequestData(url, method, postData, Encoding.UTF8);
        }

        /// <summary>
        /// 获得http请求数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postData">发送数据</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetRequestData(string url, string method, string postData, Encoding encoding)
        {
            return GetRequestData(url, method, postData, encoding, 20000);
        }

        /// <summary>
        /// 获得http请求数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postData">发送数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时值</param>
        /// <returns></returns>
        public static string GetRequestData(string url, string method, string postData, Encoding encoding, int timeout)
        {
            if (!(url.Contains("http://") || url.Contains("https://")))
                url = "http://" + url;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.Trim().ToLower();
            request.Timeout = timeout;
            request.AllowAutoRedirect = true;
            request.ContentType = "text/html";
            request.Accept = "text/html, application/xhtml+xml, */*,zh-CN";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            try
            {
                if (!string.IsNullOrEmpty(postData) && request.Method == "post")
                {
                    byte[] buffer = encoding.GetBytes(postData);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (encoding == null)
                    {
                        MemoryStream stream = new MemoryStream();
                        if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                            new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(stream, 10240);
                        else
                            response.GetResponseStream().CopyTo(stream, 10240);

                        byte[] RawResponse = stream.ToArray();
                        string temp = Encoding.Default.GetString(RawResponse, 0, RawResponse.Length);
                        Match meta = _metaregex.Match(temp);
                        string charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                        charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                        if (charter.Length > 0)
                        {
                            charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                            encoding = Encoding.GetEncoding(charter);
                        }
                        else
                        {
                            if (response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                            {
                                encoding = Encoding.GetEncoding("gbk");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(response.CharacterSet.Trim()))
                                {
                                    encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    encoding = Encoding.GetEncoding(response.CharacterSet);
                                }
                            }
                        }
                        return encoding.GetString(RawResponse);
                    }
                    else
                    {
                        StreamReader reader = null;
                        if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            using (reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                        else
                        {
                            using (reader = new StreamReader(response.GetResponseStream(), encoding))
                            {
                                try
                                {
                                    return reader.ReadToEnd();
                                }
                                catch (Exception ex)
                                {
                                    return "close";
                                }

                            }
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                return "error";
            }
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