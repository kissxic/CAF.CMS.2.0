using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services
{
	public class CommonServices : ICommonServices
	{
        private readonly Lazy<ICacheManager> _cacheManager;
        private readonly Lazy<IRequestCache> _requestCache;

        private readonly Lazy<IDbContext> _dbContext;
		private readonly Lazy<ISiteContext> _siteContext;
		private readonly Lazy<IWebHelper> _webHelper;
		private readonly Lazy<IWorkContext> _workContext;
		private readonly Lazy<IEventPublisher> _eventPublisher;
		private readonly Lazy<ILocalizationService> _localization;
		private readonly Lazy<IUserActivityService> _userActivity;
		private readonly Lazy<INotifier> _notifier;
		private readonly Lazy<IPermissionService> _permissions;
		private readonly Lazy<ISettingService> _settings;
        private readonly Lazy<ISiteService> _storeService;
        private readonly Lazy<IDisplayControl> _displayControl;
        public CommonServices(
             Lazy<ICacheManager> cacheManager,
             Lazy<IRequestCache> requestCache,
            Lazy<IDbContext> dbContext,
			Lazy<ISiteContext> siteContext,
			Lazy<IWebHelper> webHelper,
			Lazy<IWorkContext> workContext,
			Lazy<IEventPublisher> eventPublisher,
			Lazy<ILocalizationService> localization,
			Lazy<IUserActivityService> userActivity,
			Lazy<INotifier> notifier,
			Lazy<IPermissionService> permissions,
			Lazy<ISettingService> settings,
            Lazy<ISiteService> storeService,
            Lazy<IDisplayControl> displayControl)
		{
            this._cacheManager = cacheManager;
            this._requestCache = requestCache;
            this._dbContext = dbContext;
			this._siteContext = siteContext;
			this._webHelper = webHelper;
			this._workContext = workContext;
			this._eventPublisher = eventPublisher;
			this._localization = localization;
			this._userActivity = userActivity;
			this._notifier = notifier;
			this._permissions = permissions;
			this._settings = settings;
            this._storeService = storeService;
            this._displayControl = displayControl;
        }

        public ICacheManager Cache
        {
            get
            {
                return _cacheManager.Value;
            }
        }

        public IRequestCache RequestCache
        {
            get
            {
                return _requestCache.Value;
            }
        }


        public IDbContext DbContext
		{
			get
			{
				return _dbContext.Value;
			}
		}

		public ISiteContext SiteContext
		{
			get
			{
				return _siteContext.Value;
			}
		}

		public IWebHelper WebHelper
		{
			get
			{
				return _webHelper.Value;
			}
		}

		public IWorkContext WorkContext
		{
			get
			{
				return _workContext.Value;
			}
		}

		public IEventPublisher EventPublisher
		{
			get
			{
				return _eventPublisher.Value;
			}
		}

		public ILocalizationService Localization
		{
			get
			{
				return _localization.Value;
			}
		}

		public IUserActivityService UserActivity
		{
			get
			{
				return _userActivity.Value;
			}
		}

		public INotifier Notifier
		{
			get
			{
				return _notifier.Value;
			}
		}

		public IPermissionService Permissions
		{
			get 
			{
				return _permissions.Value;
			}
		}

		public ISettingService Settings
		{
			get
			{
				return _settings.Value;
			}
		}
        public ISiteService SiteService
        {
            get
            {
                return _storeService.Value;
            }
        }
        public IDisplayControl DisplayControl
        {
            get
            {
                return _displayControl.Value;
            }
        }
    }
}
