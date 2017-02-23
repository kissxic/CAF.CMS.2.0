
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
        //session���ʱ��, ��λ�Ƿ���.
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

            //�����û���Redis Session
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
        /// ��FromTicket��ȡ�û���ʶ
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
        /// ��Redis�л�ȡ�û���ʶ
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

        #region Redis �û���Ϣ����
        /// <summary>
        /// �ж��Ƿ���Redis
        /// </summary>
        private static bool RedisEnabled => CommonHelper.GetAppSetting<bool>("caf:RedisEnabled", false);

        /// <summary>
        /// ��ȡCSessionID
        /// </summary>
        /// <returns></returns>
        private string GetCSessionId()
        {

            //1, ��Request��ȡCookie
            var cookies = _httpContext.Request.Cookies;
            //2, ��Cookie�����б�������, ��ȡCSessionID
            if (null != cookies && cookies.Count > 0)
            {
                for (int i = 0; i < cookies.Count; i++)
                {
                    if (UserCookieSessionId.Equals(cookies[i].Name))
                    {
                        //��, ֱ�ӷ���
                        return cookies[i].Value;
                    }
                }

            }
            //û��, ����һ��CSessionId, ���ҷŵ�Cookie�ٷ��������.�����µ�CSessionID
            string csessionid = Guid.NewGuid().ToString().Replace("-", "");
            //���ҷŵ�Cookie��
            var cookie = new HttpCookie(UserCookieSessionId);
            cookie.HttpOnly = true;
            cookie.Value = csessionid;
            //cookie  ÿ�ζ�����, ����·��
            cookie.Path = "/";
            //int cookieExpires = 24 * 365; //ͨ��Forms����
            cookie.Expires = DateTime.Now.Add(_expirationTimeSpan);
            _httpContext.Response.Cookies.Remove(UserCookieSessionId);
            _httpContext.Response.Cookies.Add(cookie);
            return csessionid;
        }

        /// <summary>
        /// �����û���redis��  ע��: �����û�����ΪKey �û����Ƶ���value ��redis��
        /// </summary>
        /// <param name="jessionId"></param>
        /// <param name="value"></param>
        public void SetAttributerForUsername(string jessionId, string value)
        {
            var _redisContext = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IContext>();
            _redisContext.Cache.SetObject<string>(jessionId + ":USER_NAME", value, TimeSpan.FromMinutes(2 * exp));
        }

        /// <summary>
        /// ��ȡ
        /// </summary>
        /// <param name="jessionId"></param>
        /// <returns></returns>
        public string GetAttributterForUsername(string jessionId)
        {
            var _redisContext = CAF.Infrastructure.Core.EngineContext.Current.Resolve<IContext>();
            string value = _redisContext.Cache.GetObject<string>(jessionId + ":USER_NAME");
            if (null != value)
            {
                //����session����ʱ���� �û����һ������ʼ��ʱ.
                _redisContext.Cache.SetObject<string>(jessionId + ":USER_NAME", value, TimeSpan.FromMinutes(2 * exp));
                return value;
            }
            return null;
        }
        /// <summary>
        /// �Ƴ�
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