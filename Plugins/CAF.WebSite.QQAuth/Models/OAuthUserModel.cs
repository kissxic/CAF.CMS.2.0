using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.QQAuth.Models
{
    public class OAuthUser
    {
        private int _ret = -1;//状态码
        private string _msg = "";//错误信息
        private string _nickname = "";//用户昵称

        /// <summary>
        /// 状态码
        /// </summary>
        public int Ret
        {
            get { return _ret; }
            set { _ret = value; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }
    }
}