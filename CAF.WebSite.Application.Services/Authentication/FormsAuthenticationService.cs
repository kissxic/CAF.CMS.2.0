
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Web;
using System.Web.Security;
using CAF.Infrastructure.Core.Utilities;
using CachingFramework.Redis.Contracts;

namespace CAF.WebSite.Application.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private const string UserCookieSessionId = "CSESSIONID";
        //session存活时间, 单位是分钟.
        private const int exp = 30;
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly TimeSpan _expirationTimeSpan;
        private User _cachedUser;
        //  private readonly IContext _redisContext;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        /// <param name="userSettings">User settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            UserSettings userSettings,
            IUserService userService)
        {
            this._httpContext = httpContext;
            this._userService = userService;
            this._userSettings = userSettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            // this._redisContext = redisContext;
        }


        public virtual void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                _userSettings.UserNamesEnabled ? user.UserName : user.Email,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                _userSettings.UserNamesEnabled ? user.UserName : user.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            else
            {
                cookie.Expires = now.Add(_expirationTimeSpan);
            }
            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;

            //设置用户到Redis Session
            if (RedisEnabled)
                SetAttributerForUsername(GetCSessionId(), _userSettings.UserNamesEnabled ? user.UserName : user.Email);
        }

        public virtual void SignOut()
        {
            _cachedUser = null;

            FormsAuthentication.SignOut();

            if (RedisEnabled)
                Rem0veAttributterForUsername(GetCSessionId());
        }

        public virtual User GetAuthenticatedUser()
        {


            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.Active && !user.Deleted)// && user.IsRegistered())
                _cachedUser = user;

            if (user == null && RedisEnabled)
            {
                var redisUser = GetAuthenticatedUserFoRedis();
                if (redisUser != null && redisUser.Active && !redisUser.Deleted)// && user.IsRegistered())
                    _cachedUser = redisUser;
            }
            return _cachedUser;
        }
        /// <summary>
        /// 从FromTicket获取用户标识
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;
            var user = _userSettings.UserNamesEnabled
                ? _userService.GetUserByUserName(usernameOrEmail)
                : _userService.GetUserByEmail(usernameOrEmail);


            return user;
        }
        /// <summary>
        /// 从Redis中获取用户标识
        /// </summary>
        /// <returns></returns>
        private User GetAuthenticatedUserFoRedis()
        {
            var usernameOrEmail = GetAttributterForUsername(GetCSessionId());

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;
            var user = _userSettings.UserNamesEnabled
                ? _userService.GetUserByUserName(usernameOrEmail)
                : _userService.GetUserByEmail(usernameOrEmail);


            return user;
        }

        #region Redis 用户信息缓存
        /// <summary>
        /// 判断是否开启Redis
        /// </summary>
        private static bool RedisEnabled => CommonHelper.GetAppSetting<bool>("caf:RedisEnabled", false);

        /// <summary>
        /// 获取CSessionID
        /// </summary>
        /// <returns></returns>
        private string GetCSessionId()
        {

            //1, 从Request中取Cookie
            var cookies = _httpContext.Request.Cookies;
            //2, 从Cookie数据中遍历查找, 并取CSessionID
            if (null != cookies && cookies.Count > 0)
            {
                for (int i = 0; i < cookies.Count; i++)
                {
                    if (UserCookieSessionId.Equals(cookies[i].Name))
                    {
                        //有, 直接返回
                        return cookies[i].Value;
                    }
                }

            }
            //没有, 创建一个CSessionId, 并且放到Cookie再返回浏览器.返回新的CSessionID
            string csessionid = Guid.NewGuid().ToString().Replace("-", "");
            //并且放到Cookie中
            var cookie = new HttpCookie(UserCookieSessionId);
            cookie.HttpOnly = true;
            cookie.Value = csessionid;
            //cookie  每次都带来, 设置路径
            cookie.Path = "/";
            //int cookieExpires = 24 * 365; //通过Forms配置
            cookie.Expires = DateTime.Now.Add(_expirationTimeSpan);
            _httpContext.Response.Cookies.Remove(UserCookieSessionId);
            _httpContext.Response.Cookies.Add(cookie);
            return csessionid;
        }

        /// <summary>
        /// 保存用户到redis中  注册: 保存用户名作为Key 用户名称当做value 到redis中
        /// </summary>
        /// <param name="jessionId"></param>
        /// <param name="value"></param>
        public void SetAttributerForUsername(string jessionId, string value)
        {
            var _redisContext = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IContext>();
            _redisContext.Cache.SetObject<string>(jessionId + ":USER_NAME", value, TimeSpan.FromMinutes(2 * exp));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="jessionId"></param>
        /// <returns></returns>
        public string GetAttributterForUsername(string jessionId)
        {
            var _redisContext = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IContext>();
            string value = _redisContext.Cache.GetObject<string>(jessionId + ":USER_NAME");
            if (null != value)
            {
                //计算session过期时间是 用户最后一次请求开始计时.
                _redisContext.Cache.SetObject<string>(jessionId + ":USER_NAME", value, TimeSpan.FromMinutes(2 * exp));
                return value;
            }
            return null;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="jessionId"></param>
        public void Rem0veAttributterForUsername(string jessionId)
        {
            var _redisContext = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IContext>();
            _redisContext.Cache.Remove(jessionId + ":USER_NAME");

        }
        #endregion
    }
}