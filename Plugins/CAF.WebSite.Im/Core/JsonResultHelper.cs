
using CAF.Message.Distributed.Extensions;
using CAF.Message.Distributed.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Im.Core
{
    public static class JsonResultHelper
    {
        public static JsonResultModel CreateJson(object data, bool success = true, string msg = null)
        {
            return new JsonResultModel { code = success ? JsonResultType.Success : JsonResultType.Failed, data = data, msg = msg };
        }
    }
}
