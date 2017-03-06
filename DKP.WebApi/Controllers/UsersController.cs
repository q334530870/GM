using System.Net.Http;
using System.Web.Http;
using DKP.Services.Helpers;
using Newtonsoft.Json;
using DKP.Data.Models;
using System;
using DKP.Data;

namespace Papaya.WebApi.Controllers
{
    /// <summary>
    /// 测试接口
    /// </summary>
    public class UsersController : ApiController
    {
        //var token = JsonWebToken.DecodeToObject("", Variable.SecretKey) as IDictionary<string, object>;

        public HttpResponseMessage Get()
        {
            var key = Guid.NewGuid();
            var appKey = Variable.AppKey;
            var sign = Variable.MD5(key + appKey);
            var result = RequestHelper.HttpPost(Variable.RemoteUrl + "/api/server/info", "key=" + key + "&sign=" + sign);
            if (result.StartsWith("接口错误"))
            {
                return result.ToJson();
            }
            var httpModel = new HttpModel();
            try
            {
                httpModel = JsonConvert.DeserializeObject<HttpModel>(result);
                //var a = httpModel.GetValue("a");
                return httpModel.ToJson();
            }
            catch(Exception e)
            {
                return e.InnerException.Message.ToJson();
            }
        }

        /// <summary>
        /// 测试GET
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>123</returns>
        public HttpResponseMessage Get(string a,string b)
        {
            return new
            {
                code = a,
                message = b
            }.ToJson();
        }

        public void Put(int id, [FromBody]string value)
        {
        }

    }
}
