using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Admin.Models.Themes
{
    public class FilePreview : ModelBase
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 皮肤名称
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 站点
        /// </summary>
        public int SiteId { get; set; }
    }
}