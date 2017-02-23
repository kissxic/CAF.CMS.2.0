using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "����̨", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowUserImpersonation = new PermissionRecord { Name = "����ģ���û�", SystemName = "AllowUserImpersonation", Category = "Users" };
        public static readonly PermissionRecord ManageCatalog = new PermissionRecord { Name = "��Ŀ����", SystemName = "ManageCatalog", Category = "Catalog" };
        public static readonly PermissionRecord ManageChannel = new PermissionRecord { Name = "ģ�͹���", SystemName = "ManageChannel", Category = "Catalog" };
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "�û�����", SystemName = "ManageUsers", Category = "Users" };
        public static readonly PermissionRecord ManageUserRoles = new PermissionRecord { Name = "�û���ɫ", SystemName = "ManageUserRoles", Category = "Users" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "ϵͳ����", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageSites = new PermissionRecord { Name = "��վ����", SystemName = "ManageSites", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "���ʿ���", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "���Թ���", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "���ҹ���", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageArticles = new PermissionRecord { Name = "���ݹ���", SystemName = "ManageArticles", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "��ҳ����", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageClients = new PermissionRecord { Name = "����ͻ�����", SystemName = "ManageClients", Category = "Catalog Management" };
        public static readonly PermissionRecord ManageLinks = new PermissionRecord { Name = "��������", SystemName = "ManageLinks", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "��������", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "��Ϣģ��", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "�����ʾ����", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageModelTemplates = new PermissionRecord { Name = "ģ��ҳ����", SystemName = "ManageModelTemplate", Category = "Content Management" };
        public static readonly PermissionRecord ManageRegionalContents = new PermissionRecord { Name = "���ݿ�", SystemName = "ManageModelRegionalContent", Category = "Content Management" };
        public static readonly PermissionRecord ManageFeedbacks = new PermissionRecord { Name = "վ������", SystemName = "ManageFeedback", Category = "Content Management" };
        public static readonly PermissionRecord ManageMemberGrades = new PermissionRecord { Name = "��Ա�ȼ�", SystemName = "MemberGrade", Category = "Content MemberGrade" };

        
        //codehint: sm-add begin
        public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "���ҹ���", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageQuantityUnits = new PermissionRecord { Name = "����λ����", SystemName = "ManageQuantityUnits", Category = "Configuration" };
        public static readonly PermissionRecord ManageDeliveryTimes = new PermissionRecord { Name = "����ʱ�����", SystemName = "ManageDeliveryTimes", Category = "Configuration" };
        public static readonly PermissionRecord ManageContentSlider = new PermissionRecord { Name = "�õ�Ƭ", SystemName = "ManageContentSlider", Category = "Configuration" };
        public static readonly PermissionRecord ManageThemes = new PermissionRecord { Name = "�������", SystemName = "ManageThemes", Category = "Configuration" };
        //codehint: sm-add end
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "��������¼��Ȩ����", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };

        public static readonly PermissionRecord ManageMeasures = new PermissionRecord { Name = "�����ʩ", SystemName = "ManageMeasures", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "������־", SystemName = "ManageActivityLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "�����˺�", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "ϵͳ��־", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "��Ϣ����", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "ϵͳά��", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord UploadPictures = new PermissionRecord { Name = "�ϴ�ͼƬ", SystemName = "UploadPictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "ϵͳ����", SystemName = "ManageScheduleTasks", Category = "Configuration" };

        //public site permissions
        public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "��ʾ�۸�", SystemName = "DisplayPrices", Category = "PublicSite" };
        //public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "�������ﳵ", SystemName = "EnableShoppingCart", Category = "PublicSite" };
        //public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "�����ղؼ�", SystemName = "EnableWishlist", Category = "PublicSite" };
        //public static readonly PermissionRecord PublicSiteAllowNavigation = new PermissionRecord { Name = "������ʵ���", SystemName = "PublicSiteAllowNavigation", Category = "PublicSite" };


        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "�������", SystemName = "ManagePlugins", Category = "Configuration" };


        #region Vendor
        public static readonly PermissionRecord AccessSellerAdminPanel = new PermissionRecord { Name = "�̼Һ�̨", SystemName = "AccessSellerAdminPanel", Category = "Standard" };

        #endregion

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                AllowUserImpersonation,
                ManageCatalog,
                ManageChannel,
                ManageUsers,
                ManageUserRoles,
                ManageSettings,
                ManageSites,
                ManageAcl,
                ManageLanguages,
                ManageCountries,
                ManageArticles,
                ManageClients,
                ManageTopics,
                ManageLinks,
                ManageMessageTemplates,
                ManagePlugins,
                ManageWidgets,
                ManageCurrencies,
                ManageQuantityUnits,
                ManageDeliveryTimes,    //codehint: sm-add
                ManageContentSlider,    //codehint: sm-add
                ManageThemes,    //codehint: sm-add
                ManagePolls,
                ManageModelTemplates,
                ManageMeasures ,
                ManageActivityLog ,
                ManageEmailAccounts,
                ManageSystemLog ,
                ManageMessageQueue,
                ManageMaintenance ,
                UploadPictures ,
                ManageScheduleTasks ,
                ManageExternalAuthenticationMethods,
                ManageRegionalContents,
                ManageFeedbacks,
                DisplayPrices,
                AccessSellerAdminPanel,
                ManageMemberGrades,
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                 new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.SuperAdministrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageCatalog,
                        ManageChannel,
                        ManageUsers,
                        ManageUserRoles,
                        ManageSettings,
                        ManageSites,
                        ManageAcl,
                        ManageLanguages,
                        ManageCountries,
                        ManageArticles,
                        ManageClients,
                        ManageTopics,
                        ManageLinks,

                        ManageMessageTemplates,
                        ManagePlugins,
                        ManageWidgets,
                        ManageCurrencies,
                        ManageQuantityUnits,
                        ManageDeliveryTimes,    //codehint: sm-add
                        ManageContentSlider,    //codehint: sm-add
                        ManageThemes,    //codehint: sm-add
                        ManagePolls,
                        ManageModelTemplates,
                        ManageMeasures ,
                        ManageActivityLog ,
                        ManageEmailAccounts,
                        ManageSystemLog ,
                        ManageMessageQueue,
                        ManageMaintenance ,
                        UploadPictures ,
                        ManageScheduleTasks ,
                        ManageExternalAuthenticationMethods,
                        ManageRegionalContents,
                        ManageFeedbacks,
                        DisplayPrices,
                        AccessSellerAdminPanel,
                        ManageMemberGrades,
                    }
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageCatalog,
                        ManageChannel,
                        ManageUsers,
                        ManageUserRoles,
                        ManageSettings,
                        ManageSites,
                        ManageAcl,
                        ManageLanguages,
                        ManageCountries,
                        ManageArticles,
                        ManageClients,
                        ManageTopics,
                        ManageLinks,

                        ManageMessageTemplates,
                        ManagePlugins,
                        ManageWidgets,
                        ManageCurrencies,
                        ManageQuantityUnits,
                        ManageDeliveryTimes,    //codehint: sm-add
                        ManageContentSlider,    //codehint: sm-add
                        ManageThemes,    //codehint: sm-add
                        ManagePolls,
                        ManageModelTemplates,
                        ManageMeasures ,
                        ManageActivityLog ,
                        ManageEmailAccounts,
                        ManageSystemLog ,
                        ManageMessageQueue,
                        ManageMaintenance ,
                        UploadPictures ,
                        ManageScheduleTasks ,
                        ManageExternalAuthenticationMethods,
                        ManageRegionalContents,
                        ManageFeedbacks,
                        DisplayPrices,
                        ManageMemberGrades,
                    }
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.ForumModerators,
                    //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Guests,
                                       //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Registered,
                                       //PermissionRecords = new[] 
                    //{
                        
                    //}
                },
                  new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Vendors,
                    PermissionRecords = new[]
                    {
                        AccessSellerAdminPanel,
                        ManageCatalog,
                        ManageArticles,
                    }
                  }
            };
        }
    }
}