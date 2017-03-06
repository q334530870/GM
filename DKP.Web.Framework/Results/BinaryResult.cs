using System.IO;
using System.Web;
using System.Web.Mvc;

namespace DKP.Web.Framework.Results
{

    /// <summary>
    /// 向浏览器输出binary的ActionResult
    /// </summary>
    public class BinaryResult : ActionResult
    {
        private readonly string _filePath;
        private readonly string _contentType;
        private readonly byte[] _contentBytes;
        private readonly string _filename;


        public BinaryResult(string filePath, string contentType, string filename)
        {
            _filePath = filePath;
            _contentType = contentType;
            _filename = filename;
        }
        public BinaryResult(byte[] contentBytes, string contentType, string filename)
        {
            _contentBytes = contentBytes;
            _contentType = contentType;
            _filename = filename;
        }
        public BinaryResult(string filePath, string filename)
        {
            _filePath = filePath;
            _filename = filename;
        }
        public BinaryResult(byte[] contentBytes, string filename)
        {
            _contentBytes = contentBytes;
            _filename = filename;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.Public);
            if (string.IsNullOrEmpty(this._contentType))
            {
                response.ContentType = this._contentType;
            }
            response.AddHeader("Content-Disposition", "attachment;filename=" + _filename);
            response.AddHeader("Cache-Control", "no-cache");

            var bytes = this._contentBytes;
            if (!string.IsNullOrEmpty(_filePath))
            {
                bytes = File.ReadAllBytes(context.HttpContext.Server.MapPath(_filePath));
            }

            using (var stream = new MemoryStream(bytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }
    }
}