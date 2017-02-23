using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class JsonpResult : JsonResult
    {
        private static readonly string JsonpCallbackName = "callback";
        private static readonly string CallbackApplicationType = "application/json";

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="context"/> parameter is null.</exception>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                  String.Equals(context.HttpContext.Request.HttpMethod, "GET"))
            {
                throw new InvalidOperationException();
            }
            var response = context.HttpContext.Response;
            if (!String.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = CallbackApplicationType;
            if (ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (Data != null)
            {
                String buffer;
                var request = context.HttpContext.Request;

                if (request[JsonpCallbackName] != null)
                    buffer = String.Format("{0}({1})", request[JsonpCallbackName], JsonConvert.SerializeObject(Data));
                else
                    buffer = JsonConvert.SerializeObject(Data);
                response.Write(buffer);
            }
        }
    }
}
