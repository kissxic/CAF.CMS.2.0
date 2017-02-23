using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Net;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        private static bool? s_optimizedCompilationsEnabled = null;
        private static AspNetHostingPermissionLevel? s_trustLevel = null;
        private static readonly Regex s_staticExts = new Regex(@"(.*?)\.(css|js|png|jpg|jpeg|gif|bmp|html|htm|xml|pdf|doc|xls|rar|zip|ico|eot|svg|ttf|woff|otf|axd|ashx|less)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex s_htmlPathPattern = new Regex(@"(?<=(?:href|src)=(?:""|'))(?!https?://)(?<url>[^(?:""|')]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_cssPathPattern = new Regex(@"url\('(?<url>.+)'\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static ConcurrentDictionary<int, string> s_safeLocalHostNames = new ConcurrentDictionary<int, string>();

        private readonly HttpContextBase _httpContext;
        private bool? _isCurrentConnectionSecured;
        private string _siteHost;
        private string _siteHostSsl;
        private bool? _appPathPossiblyAppended;
        private bool? _appPathPossiblyAppendedSsl;

        private Site _currentSiteRegister;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            if (_httpContext != null &&
                _httpContext.Request != null &&
                _httpContext.Request.UrlReferrer != null)
                referrerUrl = _httpContext.Request.UrlReferrer.ToString();

            return referrerUrl;
        }

        /// <summary>
        /// ��ȡ�ͻ���ip��ַ
        /// </summary>
        /// <param name="transparent">�Ƿ�ʹ���˴���</param>
        /// <returns>ip��ַ</returns>
        public virtual string GetCurrentIpAddress(bool transparent = false)
        {
            string ip = string.Empty;
            if (_httpContext != null && _httpContext.Request != null)
            {
                if (transparent)
                {
                    if (_httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                    {
                        ip = _httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                }
                if (string.IsNullOrWhiteSpace(ip))
                {
                    if (_httpContext.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        ip = _httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else
                    {
                        ip = _httpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                }
                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = _httpContext.Request.UserHostAddress.EmptyNull();
                }

            }

            return ip;
        }


        /// <summary>
        /// ����Ƿ�������IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public virtual bool IsLocalIp(string ipAddress)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                if (ipAddress.StartsWith("192.168.")
                    || ipAddress.StartsWith("172.")
                    || ipAddress.StartsWith("10."))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            string url = string.Empty;
            if (_httpContext == null || _httpContext.Request == null)
                return url;

            if (includeQueryString)
            {
                bool appPathPossiblyAppended;
                string siteHost = GetSiteHost(useSsl, out appPathPossiblyAppended).TrimEnd('/');

                string rawUrl = string.Empty;
                if (appPathPossiblyAppended)
                {
                    string temp = _httpContext.Request.AppRelativeCurrentExecutionFilePath.TrimStart('~');
                    rawUrl = temp;
                }
                else
                {
                    rawUrl = _httpContext.Request.RawUrl;
                }

                url = siteHost + rawUrl;
            }
            else
            {
                if (_httpContext.Request.Url != null)
                {
                    url = _httpContext.Request.Url.GetLeftPart(UriPartial.Path);
                }
            }

            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!_isCurrentConnectionSecured.HasValue)
            {
                _isCurrentConnectionSecured = false;
                if (_httpContext != null && _httpContext.Request != null)
                {
                    _isCurrentConnectionSecured = _httpContext.Request.IsSecureConnection();
                }
            }

            return _isCurrentConnectionSecured.Value;
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name)
        {
            string result = string.Empty;

            try
            {
                if (_httpContext != null && _httpContext.Request != null)
                {
                    if (_httpContext.Request.ServerVariables[name] != null)
                    {
                        result = _httpContext.Request.ServerVariables[name];
                    }
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        private string GetHostPart(string url)
        {
            var uri = new Uri(url);
            var host = uri.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped);
            return host;
        }


        /// <summary>
        /// Gets site host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <param name="appPathPossiblyAppended">
        ///     <c>true</c> when the host url had to be resolved from configuration, 
        ///     where a possible folder name may have been specified (e.g. www.mysite.com/SHOP)
        /// </param>
        /// <returns>SiteRegister host location</returns>
        private string GetSiteHost(bool useSsl, out bool appPathPossiblyAppended)
        {
            string cached = useSsl ? _siteHostSsl : _siteHost;
            if (cached != null)
            {
                appPathPossiblyAppended = useSsl ? _appPathPossiblyAppendedSsl.Value : _appPathPossiblyAppended.Value;
                return cached;
            }

            appPathPossiblyAppended = false;
            var result = "";
            var httpHost = ServerVariables("HTTP_HOST");

            if (httpHost.HasValue())
            {
                result = "http://" + httpHost.EnsureEndsWith("/");
            }

            if (!DataSettings.DatabaseIsInstalled())
            {
                if (useSsl)
                {
                    // Secure URL is not specified.
                    // So a site owner wants it to be detected automatically.
                    result = result.Replace("http:/", "https:/");
                }
            }
            else
            {
                //let's resolve IWorkContext  here.
                //Do not inject it via contructor because it'll cause circular references

                if (_currentSiteRegister == null)
                {
                    ISiteContext siteContext;
                    if (EngineContext.Current.ContainerManager.TryResolve<ISiteContext>(null, out siteContext)) // Unit test safe!
                    {
                        _currentSiteRegister = siteContext.CurrentSite;
                        if (_currentSiteRegister == null)
                            throw new Exception("Current site cannot be loaded");
                    }
                }

                if (_currentSiteRegister != null)
                {
                    //var securityMode = _currentSiteRegister.GetSecurityMode(useSsl);

                    if (httpHost.IsEmpty())
                    {
                        //HTTP_HOST variable is not available.
                        //It's possible only when HttpContext is not available (for example, running in a schedule task)
                        result = _currentSiteRegister.Url.EnsureEndsWith("/");

                        appPathPossiblyAppended = true;
                    }

                    //if (useSsl)
                    //{
                    //    if (securityMode == HttpSecurityMode.SharedSsl)
                    //    {
                    //        // Secure URL for shared ssl specified. 
                    //        // So a site owner doesn't want it to be resolved automatically.
                    //        // In this case let's use the specified secure URL
                    //        result = _currentSiteRegister.SecureUrl.EmptyNull();

                    //        if (!result.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    //        {
                    //            result = "https://" + result;
                    //        }

                    //        appPathPossiblyAppended = true;
                    //    }
                    //    else
                    //    {
                    //        // Secure URL is not specified.
                    //        // So a site owner wants it to be resolved automatically.
                    //        result = result.Replace("http:/", "https:/");
                    //    }
                    //}
                    //else // no ssl
                    //{
                    //    if (securityMode == HttpSecurityMode.SharedSsl)
                    //    {
                    //        // SSL is enabled in this site and shared ssl URL is specified.
                    //        // So a site owner doesn't want it to be resolved automatically.
                    //        // In this case let's use the specified non-secure URL

                    //        result = _currentSiteRegister.Url;
                    //        appPathPossiblyAppended = true;
                    //    }
                    //}
                }
            }

            // cache results for request
            result = result.EnsureEndsWith("/").ToLowerInvariant();
            if (useSsl)
            {
                _siteHostSsl = result;
                _appPathPossiblyAppendedSsl = appPathPossiblyAppended;
            }
            else
            {
                _siteHost = result;
                _appPathPossiblyAppended = appPathPossiblyAppended;
            }

            return result;
        }

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <returns>SiteRegister location</returns>
        public virtual string GetSiteLocation()
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetSiteLocation(useSsl);
        }

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>SiteRegister location</returns>
        public virtual string GetSiteLocation(bool useSsl)
        {
            //return HostingEnvironment.ApplicationVirtualPath;

            bool appPathPossiblyAppended;
            string result = GetSiteHost(useSsl, out appPathPossiblyAppended);

            if (result.EndsWith("/"))
            {
                result = result.Substring(0, result.Length - 1);
            }

            if (_httpContext != null && _httpContext.Request != null)
            {
                var appPath = _httpContext.Request.ApplicationPath;
                if (!appPathPossiblyAppended && !result.EndsWith(appPath, StringComparison.OrdinalIgnoreCase))
                {
                    // in a shared ssl scenario the user defined https url could contain
                    // the app path already. In this case we must not append.
                    result = result + appPath;
                }
            }

            if (!result.EndsWith("/"))
            {
                result += "/";
            }

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are - among others - the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public virtual bool IsStaticResource(HttpRequest request)
        {
            return IsStaticResourceRequested(new HttpRequestWrapper(request));
        }

        public static bool IsStaticResourceRequested(HttpRequest request)
        {
            Guard.ArgumentNotNull(() => request);
            return s_staticExts.IsMatch(request.Path);
        }

        public static bool IsStaticResourceRequested(HttpRequestBase request)
        {
            // unit testable
            Guard.ArgumentNotNull(() => request);
            return s_staticExts.IsMatch(request.Path);
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            return CommonHelper.MapPath(path, false);
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            // TODO: routine should not return a query string in lowercase (unless the caller is telling him to do so).
            url = url.EmptyNull().ToLower();
            queryStringModification = queryStringModification.EmptyNull().ToLower();

            string curAnchor = null;

            var hsIndex = url.LastIndexOf('#');
            if (hsIndex >= 0)
            {
                curAnchor = url.Substring(hsIndex);
                url = url.Substring(0, hsIndex);
            }

            var parts = url.Split(new[] { '?' });
            var current = new QueryString(parts.Length == 2 ? parts[1] : "");
            var modify = new QueryString(queryStringModification);

            foreach (var nv in modify.AllKeys)
            {
                current.Add(nv, modify[nv], true);
            }

            var result = "{0}{1}{2}".FormatCurrent(parts[0], current.ToString(), anchor.NullEmpty() == null ? (curAnchor == null ? "" : "#" + curAnchor.ToLower()) : "#" + anchor.ToLower());
            return result;
        }

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            var parts = url.EmptyNull().ToLower().Split(new[] { '?' });
            var current = new QueryString(parts.Length == 2 ? parts[1] : "");

            if (current.Count > 0 && queryString.HasValue())
            {
                current.Remove(queryString);
            }

            var result = "{0}{1}".FormatCurrent(parts[0], current.ToString());
            return result;
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            string queryParam = null;
            if (_httpContext != null && _httpContext.Request.QueryString[name] != null)
                queryParam = _httpContext.Request.QueryString[name];

            if (!String.IsNullOrEmpty(queryParam))
                return queryParam.Convert<T>();

            return default(T);
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether </param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        public virtual void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "", bool aggressive = false)
        {
            HttpRuntime.UnloadAppDomain();

            if (aggressive)
            {
                TryWriteBinFolder();
            }
            else
            {
                // without this, MVC may fail resolving controllers for newly installed plugins after IIS restart
                Thread.Sleep(250);
            }

            // If setting up plugins requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (_httpContext != null && makeRedirect)
            {
                if (_httpContext.Request.RequestType == "GET")
                {
                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = GetThisPageUrl(true);
                    }
                    _httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
                }
                else
                {
                    // Don't redirect posts...
                    _httpContext.Response.ContentType = "text/html";
                    _httpContext.Response.WriteFile("~/refresh.html");
                    _httpContext.Response.End();
                }
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteGlobalAsax()
        {
            try
            {
                //When a new plugin is dropped in the Plugins folder and is installed into CAF.Infrastructure.Core.NET, 
                //even if the plugin has registered routes for its controllers, 
                //these routes will not be working as the MVC framework can't
                //find the new controller types in order to instantiate the requested controller. 
                //That's why you get these nasty errors 
                //i.e "Controller does not implement IController".
                //The solution is to touch the 'top-level' global.asax file
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteBinFolder()
        {
            try
            {
                var binMarker = MapPath("~/bin/HostRestart");
                Directory.CreateDirectory(binMarker);

                using (var stream = File.CreateText(Path.Combine(binMarker, "marker.txt")))
                {
                    stream.WriteLine("Restart on '{0}'", DateTime.UtcNow);
                    stream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool OptimizedCompilationsEnabled
        {
            get
            {
                if (!s_optimizedCompilationsEnabled.HasValue)
                {
                    var section = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
                    s_optimizedCompilationsEnabled = section.OptimizeCompilations;
                }

                return s_optimizedCompilationsEnabled.Value;
            }
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine(HttpContextBase context)
        {
            //we accept HttpContext instead of HttpRequest and put required logic in try-catch block
            //more info: http://www.nopcommerce.com/boards/t/17711/unhandled-exception-request-is-not-available-in-this-context.aspx
            if (context == null)
                return false;

            bool result = false;
            try
            {
                if (context.Request.GetType().ToString().Contains("Fake"))	// codehint: sm-add
                    return false;

                result = context.Request.Browser.Crawler;
                if (!result)
                {
                    //put any additional known crawlers in the Regex below for some custom validation
                    //var regEx = new Regex("Twiceler|twiceler|BaiDuSpider|baduspider|Slurp|slurp|ask|Ask|Teoma|teoma|Yahoo|yahoo");
                    //result = regEx.Match(request.UserAgent).Success;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
            return result;
        }

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public virtual bool IsPostBeingDone
        {
            get
            {
                if (_httpContext.Items["sm.IsPOSTBeingDone"] == null)
                    return false;
                return Convert.ToBoolean(_httpContext.Items["sm.IsPOSTBeingDone"]);
            }
            set
            {
                _httpContext.Items["sm.IsPOSTBeingDone"] = value;
            }
        }

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!s_trustLevel.HasValue)
            {
                //set minimum
                s_trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal 
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        s_trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return s_trustLevel.Value;
        }

        /// <summary>
        /// Prepends protocol and host to all (relative) urls in a html string
        /// </summary>
        /// <param name="html">The html string</param>
        /// <param name="request">Request object</param>
        /// <returns>The transformed result html</returns>
        /// <remarks>
        /// All html attributed named <c>src</c> and <c>href</c> are affected, also occurences of <c>url('path')</c> within embedded stylesheets.
        /// </remarks>
        public static string MakeAllUrlsAbsolute(string html, HttpRequestBase request)
        {
            Guard.ArgumentNotNull(() => request);

            if (request.Url == null)
            {
                return html;
            }

            return MakeAllUrlsAbsolute(html, request.Url.Scheme, request.Url.Authority);
        }

        /// <summary>
        /// Prepends protocol and host to all (relative) urls in a html string
        /// </summary>
        /// <param name="html">The html string</param>
        /// <param name="protocol">The protocol to prepend, e.g. <c>http</c></param>
        /// <param name="host">The host name to prepend, e.g. <c>www.mysite.com</c></param>
        /// <returns>The transformed result html</returns>
        /// <remarks>
        /// All html attributed named <c>src</c> and <c>href</c> are affected, also occurences of <c>url('path')</c> within embedded stylesheets.
        /// </remarks>
        public static string MakeAllUrlsAbsolute(string html, string protocol, string host)
        {
            Guard.ArgumentNotEmpty(() => html);
            Guard.ArgumentNotEmpty(() => protocol);
            Guard.ArgumentNotEmpty(() => host);

            string baseUrl = string.Format("{0}://{1}", protocol, host.TrimEnd('/'));

            MatchEvaluator evaluator = (match) =>
            {
                var url = match.Groups["url"].Value;
                return "{0}{1}".FormatCurrent(baseUrl, url.EnsureStartsWith("/"));
            };

            html = s_htmlPathPattern.Replace(html, evaluator);
            html = s_cssPathPattern.Replace(html, evaluator);

            return html;
        }

        /// <summary>
        /// Prepends protocol and host to the given (relative) url
        /// </summary>
        public static string GetAbsoluteUrl(string url, HttpRequestBase request)
        {
            Guard.ArgumentNotEmpty(() => url);
            Guard.ArgumentNotNull(() => request);

            if (request.Url == null)
            {
                return url;
            }

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.StartsWith("~"))
            {
                url = VirtualPathUtility.ToAbsolute(url);
            }

            url = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url);
            return url;
        }

        public static HttpWebRequest CreateHttpRequestForSafeLocalCall(Uri requestUri)
        {
            Guard.ArgumentNotNull(() => requestUri);

            var safeHostName = GetSafeLocalHostName(requestUri);

            var uri = requestUri;

            if (!requestUri.Host.Equals(safeHostName, StringComparison.OrdinalIgnoreCase))
            {
                var url = String.Format("{0}://{1}{2}",
                    requestUri.Scheme,
                    requestUri.IsDefaultPort ? safeHostName : safeHostName + ":" + requestUri.Port,
                    requestUri.PathAndQuery);
                uri = new Uri(url);
            }

            var request = WebRequest.CreateHttp(uri);
            request.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
            request.ServicePoint.Expect100Continue = false;
            request.UserAgent = "SmartStore.NET {0}".FormatInvariant(WorkVersion.CurrentFullVersion);

            return request;
        }

        private static string GetSafeLocalHostName(Uri requestUri)
        {
            return s_safeLocalHostNames.GetOrAdd(requestUri.Port, (port) =>
            {
                // first try original host
                if (TestHost(requestUri, requestUri.Host, 5000))
                {
                    return requestUri.Host;
                }

                // try loopback
                var hostName = Dns.GetHostName();
                var hosts = new List<string> { "localhost", hostName, "127.0.0.1" };
                foreach (var host in hosts)
                {
                    if (TestHost(requestUri, host, 500))
                    {
                        return host;
                    }
                }

                // try local IP addresses
                hosts.Clear();
                var ipAddresses = Dns.GetHostAddresses(hostName).Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());
                hosts.AddRange(ipAddresses);

                foreach (var host in hosts)
                {
                    if (TestHost(requestUri, host, 500))
                    {
                        return host;
                    }
                }

                // None of the hosts are callable. WTF?
                return requestUri.Host;
            });
        }
        private static bool TestHost(Uri originalUri, string host, int timeout)
        {
            var url = String.Format("{0}://{1}/taskscheduler/noop",
                originalUri.Scheme,
                originalUri.IsDefaultPort ? host : host + ":" + originalUri.Port);
            var uri = new Uri(url);

            var request = WebRequest.CreateHttp(uri);
            request.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
            request.ServicePoint.Expect100Continue = false;
            request.UserAgent = "CAF.WebSite";
            request.Timeout = timeout;

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch
            {
                // try the next host
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }

            return false;
        }
        private class SiteRegisterHost
        {
            public string Host { get; set; }
            public bool ExpectingDirtySecurityChannelMove { get; set; }
        }

    }
}