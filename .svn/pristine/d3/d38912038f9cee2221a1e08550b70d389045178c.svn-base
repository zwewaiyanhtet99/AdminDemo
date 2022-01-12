using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ABankAdmin.Utils
{
    public class JsonErrorResult : JsonResult
    {
        private readonly HttpStatusCode _statusCode;

        public JsonErrorResult(object json) : this(json, HttpStatusCode.InternalServerError)
        {
        }

        public JsonErrorResult(object json, HttpStatusCode statusCode)
        {
            Data = json;
            _statusCode = statusCode;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_statusCode;
            base.ExecuteResult(context);
        }
    }
}