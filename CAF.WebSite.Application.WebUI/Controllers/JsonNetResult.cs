using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
        }
        public JsonNetResult(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet, string contentType = null, Encoding contentEncoding = null)
        {
            this.Data = data;
            this.JsonRequestBehavior = behavior;
            this.ContentEncoding = contentEncoding;
            this.ContentType = contentType;
        }


        public override void ExecuteResult(ControllerContext context)
        {


            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;
            var jsonSerizlizerSetting = new JsonSerializerSettings();
            //设置首字母小写
            jsonSerizlizerSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //设置日期的格式为：yyyy-MM-dd
            jsonSerizlizerSetting.DateFormatString = "yyy-MM-dd";
            var json = JsonConvert.SerializeObject(Data, Formatting.None, jsonSerizlizerSetting);

            response.Write(json);


            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


        }
    }

}
