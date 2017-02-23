using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.XLAuth.Models
{
    public class OAuthUser
    {
        public string id { get; set; }

        public string name { get; set; }

        public string screen_name { get; set; }

        public string profile_image_url { get; set; }
        /// <summary>
        /// 所在地区
        /// </summary>
        public string location { get; set; }

        public string gender { get; set; }

        public byte uType { get; set; }
    }
}