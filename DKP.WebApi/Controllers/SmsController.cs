using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DKP.Core.Infrastructure;
using DKP.Services.Security;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections;

namespace Papaya.WebApi.Controllers
{
    public class SmsController : ApiController
    {
        //public HttpResponseMessage Get(string mobile)
        //{
        //    var userService = EngineContext.Current.Resolve<IUserService>();
        //    if (userService.CheckMobile(mobile))
        //    {
        //        return new
        //        {
        //            code = "error",
        //            message = "对不起，手机号码已被注册"
        //        }.ToJson();
        //    }

        //    var radom = new Random();
        //    var code = radom.Next(100000, 999999).ToString();
        //    var param = new Dictionary<string, string>();
        //    param.Add("code", code);
        //    param.Add("product", "京西贷");

        //    var mv = new MobileVaild();
        //    mv.Mobile = mobile;
        //    mv.Code = code;

        //    ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", "23332442", "dc1b7995e766e1f7467061e53157bd9f");
        //    AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
        //    req.SmsType = "normal";
        //    req.SmsFreeSignName = "注册验证";
        //    req.SmsParam = (new JavaScriptSerializer()).Serialize(param);
        //    req.RecNum = mobile;
        //    req.SmsTemplateCode = "SMS_6780186";
        //    AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);

        //    if (rsp.IsError)
        //    {
        //        return new { code = "error", message = rsp.SubErrMsg }.ToJson();
        //    }

        //    mv.SendTime = DateTime.Now;

        //    if (Val.hs.Contains(mobile))
        //    {
        //        Val.hs.Remove(mobile);
        //    }
        //    Val.hs.Add(mobile, mv);

        //    return new { code = "success" }.ToJson();
        //}

        //public HttpResponseMessage Post([FromBody]JObject jb)
        //{
        //    dynamic json = jb;
        //    string mobile = json.mobile;
        //    string code = json.code;

        //    if (Val.hs != null && Val.hs[mobile] != null)
        //    {
        //        var result = Val.hs[mobile] as MobileVaild;
        //        if (result.Code != code)
        //        {
        //            return new { code = "error", message = "验证码不正确" }.ToJson();
        //        }

        //        Val.hs.Remove(mobile);
        //        return new { code = "success" }.ToJson();
        //    }
        //    else
        //    {
        //        return new { code = "error", message = "验证码已过期" }.ToJson();
        //    }
        //}
    }

    public static class Val
    {
        public static Hashtable hs = new Hashtable();
    }

    class MobileVaild
    {
        public string Mobile { get; set; }

        public string Code { get; set; }

        public DateTime SendTime { get; set; }
    }
}
