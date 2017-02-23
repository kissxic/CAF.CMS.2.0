using System;

namespace CAF.WebSite.Application.WebUI
{
	public static class CacheKeyCollection
	{
	 

		public static string Manager(long managerId)
		{
			return string.Format("Cache-Manager-{0}", managerId);
		}

		public static string ManagerLoginError(string username)
		{
			return string.Format("Cache-Manager-Login-{0}", username);
		}

		public static string Member(long memberId)
		{
			return string.Format("Cache-Member-{0}", memberId);
		}

		public static string MemberFindPassWordCheck(string username, string pluginId)
		{
			return string.Format("Cache-Member-PassWord-{0}-{1}", username, pluginId);
		}

		public static string MemberLoginError(string username)
		{
			return string.Format("Cache-Member-Login-{0}", username);
		}

        public static string MemberSendPhoneCodeCheckTime(string phone)
        {
            return string.Format("Cache-CheckTime-{0}", phone);
        }


        public static string SessionVerifyCode(string codeName)
        {
            return string.Format("VerifyCode-{0}", codeName);
        }
    }
}