using System;
using System.Web.Mvc;
using CAF.WebSite.Im.Models;
using CAF.WebSite.Im;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using System.Web;
using CAF.WebSite.Im.Core;
using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Core;
using CAF.Message.Distributed.Extensions.Models;

namespace CAF.WebSite.Im.Controllers
{

    public class IMController : PluginControllerBase
    {
        private readonly IMSettings _imSettings;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly ILayIMCache _layIMCache;
        public IMController(IMSettings imSettings,
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            ILayIMCache layIMCache)
        {
            this._imSettings = imSettings;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._layIMCache = layIMCache;
        }

        public ActionResult Configure()
        {
            var model = new IMModel();
            model.Enabled = _imSettings.Enabled;
            model.MongoServerSettings = _imSettings.MongoServerSettings;
            model.RabbitMqUri = _imSettings.RabbitMqUri;
            model.RabbitUserName = _imSettings.RabbitUserName;
            model.RabbitPassword = _imSettings.RabbitPassword;
            return View(model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(IMModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _imSettings.Enabled = model.Enabled;
            _imSettings.MongoServerSettings = model.MongoServerSettings;
            _imSettings.RabbitMqUri = model.RabbitMqUri;
            _imSettings.RabbitUserName = model.RabbitUserName;
            _imSettings.RabbitPassword = model.RabbitPassword;
            _settingService.SaveSetting(_imSettings);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object model)
        {
            return base.View();
        }
        #region 获取基本信息和群信息
        [HttpGet]
        [ActionName("base")]
        /// <summary>
        /// 获取基本列表 layimapi/base
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JsonResult GetBaseList(int? userid)
        {
            var baseUserInfo = this._layIMCache.GetUserBaseInfo();
            var result = JsonResultHelper.CreateJson(baseUserInfo, true);
            return new JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("member")]
        /// <summary>
        /// 获取群组员信息
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public JsonResult GetMembersList(int id)
        {
            //  var result = LayimUserBLL.Instance.GetGroupMembers(id);
            var result = JsonResultHelper.CreateJson("", true);
            return new JsonNetResult(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 上传文件和图片

        [HttpPost]
        [ActionName("upload_img")]
        public JsonResult UploadImg(HttpPostedFileBase file, int uid = 0)
        {
            //var upload = FileUploadHelper.Upload(file, Server.MapPath("/upload/"), true);

            //if (uid > 0 && upload.code == 0)
            //{
            //    var path = upload.data.GetType().GetProperty("src").GetValue(upload.data).ToString();
            //    Task.Run(() =>
            //    {
            //        //更新用户皮肤
            //        LayimUserBLL.Instance.UpdateUserSkin(uid, path);
            //    });

            //}
            //   var result = Json(upload, JsonRequestBehavior.DenyGet);
            var result = JsonResultHelper.CreateJson("", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ActionName("upload_file")]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            //  return Json(FileUploadHelper.Upload(file, Server.MapPath("/upload/"), false), JsonRequestBehavior.DenyGet);
            var result = JsonResultHelper.CreateJson("", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 好友或者用户群组申请
        [HttpPost]
        [ActionName("apply")]
        public JsonResult AddFriendOrJoinInGroup(string userid, string targetid, string other = "", bool isfriend = true)
        {
            if (userid == targetid) { throw new ArgumentException("userid can't equal with targetid"); }

            _layIMCache.ApplyToClient(new ApplyEntity()
            {
                applytime = DateTime.UtcNow,
                applytype = (int)ApplyType.ClientToClient,
                userid = userid,
                targetid = targetid,
                other = other,
            });
            var toUserIds = new string[] { targetid };
            if (toUserIds.Length > 0)
            {
                string userIdsStr = string.Join(",", toUserIds);
                //给对方发送好友消息 传targetid
                HubServerHelper.SendMessage(new ApplySendedMessage { msg = isfriend ? "您有1条加好友消息" : "您有1条加群消息" }, userIdsStr, ChatToClientType.ApplySendedToClient, !isfriend);
            }
            return Json(JsonResultHelper.CreateJson(toUserIds, true), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取需要我处理的好友请求
        [HttpGet]
        [ActionName("myapply")]
        public JsonResult GetUserNeedHandleApply(int? userid)
        {
            //var result = LayIMUserJoinBLL.Instance.GetUserNeedHandleApply(userid);
            //return Json(result, JsonRequestBehavior.AllowGet);
            var result = JsonResultHelper.CreateJson("", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 创建用户群
        [HttpPost]
        [ActionName("create")]
        public JsonResult CreateGroup(string n, string d, int uid)
        {
            //var result = LayimUserBLL.Instance.CreateGroup(n, d, uid);
            //var group = result.data as UserGroupCreatedMessage;
            ////向客户端推送创建成功的消息
            //HubServerHelper.SendMessage(group, uid.ToString(), ChatToClientType.GroupCreatedToClient);
            //return Json(result, JsonRequestBehavior.DenyGet);
            var result = JsonResultHelper.CreateJson("", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取用户申请消息
        [HttpGet]
        [ActionName("msg")]
        public JsonResult GetUserApplyMessage(int? userid)
        {
            //var result = LayimUserBLL.Instance.GetUserApplyMessage(userid);
            //return Json(result, JsonRequestBehavior.AllowGet);
            var result = JsonResultHelper.CreateJson("", true);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 处理用户的请求
        [HttpPost]
        [ActionName("handle")]
        public JsonResult HandleApply(int applyid, int userid, short result, string reason = "")
        {
            //var res = LayIMUserJoinBLL.Instance.HandleApply(applyid, userid, result, reason);
            //var msg = res.data as ApplyHandledMessgae;
            ////推送消息
            //HubServerHelper.SendMessage(msg);
            //return Json(res, JsonRequestBehavior.DenyGet);
            var res = JsonResultHelper.CreateJson("", true);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}