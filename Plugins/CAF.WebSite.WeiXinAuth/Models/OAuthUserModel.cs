using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.WeiXinAuth.Models
{
    public class WeiXinAccessTokenResult
    {
        public WeiXinAccessTokenModel SuccessResult { get; set; }
        public bool Result { get; set; }

        public WeiXinErrorMsg ErrorResult { get; set; }

    }
    /// <summary>
    /// 通过code获取access_token 请求成功的实体
    /// </summary>
    public class WeiXinAccessTokenModel
    {
        /// <summary>
        /// 接口调用凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 用户刷新access_token
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public string scope { get; set; }
    }

    /// <summary>
    /// 微信错误访问的情况 
    /// </summary>
    public class WeiXinErrorMsg
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误提示消息
        /// </summary>
        public string errmsg { get; set; }
    }

    /// <summary>
    /// 获取微信用户信息
    /// </summary>
    public class WeiXinUserInfoResult
    {
        /// <summary>
        /// 微信用户信息
        /// </summary>
        public WeiXinUserInfo UserInfo { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public WeiXinErrorMsg ErrorMsg { get; set; }
    }

    public class WeiXinUserInfo
    {
        /// <summary>
        /// 用户的唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 用户个人资料填写的省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 普通用户个人资料填写的城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 国家，如中国为CN
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string headimgurl { get; set; }
        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
        /// </summary>
        public string[] privilege { get; set; }
    }
}