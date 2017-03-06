using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using DKP.Core;
using DKP.Core.Infrastructure;

namespace DKP.Web.Framework
{
    public static class LocalizationHelpers
    {
        /// <summary>
        /// 在 Html 中直接使用
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Lang(this HtmlHelper htmlhelper, string key)
        {
            return GetLangString(key);
        }

        /// <summary>
        /// 在js中使用
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string LangOutJsVar(this HtmlHelper htmlhelper, string key)
        {
            string langstr = GetLangString(key);
            return string.Format("var {0} = '{1}'", key, langstr);
        }

        /// <summary>
        /// Controller中使用
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Lang(this Controller control, string key)
        {
            return GetLangString(key);
        }

        public static string Lang(this HttpContextBase httpContext, string key)
        {
            return GetLangString(key);
        }

        public static string Lang(this Control control, string key)
        {
            return GetLangString(key);
        }


        private static ILanguageProvider _langProvider;

        private static string GetLangString(string key)
        {
            string lang = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage;
            /*加载语言，只在第一次使用时加载*/
            if (_langProvider == null)
            {
                _langProvider = new LanguageProvider();
            }
            return _langProvider.GetLanguage(lang, key);
        }
    }

    /// <summary>
    /// 语言提供者接口
    /// </summary>
    public interface ILanguageProvider
    {
        /// <summary>
        /// 查询相应语言值
        /// </summary>
        /// <param name="lang">语言名称</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        string GetLanguage(string lang, string key);
    }

    /// <summary>
    /// 语言提供者
    /// </summary>
    public class LanguageProvider : ILanguageProvider
    {
        /// <summary>
        /// 存放语言文件的虚拟路径(可自行修改为动态路径)
        /// </summary>
        private const string LangPath = "~/Resources/";

        /// <summary>
        /// 语言文件的后缀
        /// </summary>
        private const string ExtName = ".resx";

        /// <summary>
        /// 语言集（key是语言的名称，如zh-cn，en-us等）
        /// </summary>
        private readonly Dictionary<string, ILanguage> _langResources;

        public LanguageProvider()
            : this(null)
        {
        }

        public LanguageProvider(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = HttpContext.Current.Server.MapPath(LangPath);
            _langResources = new Dictionary<string, ILanguage>();
            string[] files = Directory.GetFiles(filePath);
            files = files.Where(c => c.EndsWith(ExtName)).ToArray();
            foreach (var file in files)
            {
                ILanguage reader = new Language();
                reader.Read(file);
                _langResources.Add(GetKey(file), reader);
            }
            /*监视语言文件，如果有文件更新就更新相应的语言*/
            Watch(filePath);
        }

        /// <summary>
        /// 查询相应语言值
        /// </summary>
        /// <param name="lang">语言名称</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string GetLanguage(string lang, string key)
        {
            var r = _langResources.ContainsKey(lang) ? _langResources[lang] : _langResources.First().Value;
            return r.Get(key);
        }

        /// <summary>
        /// 从文件路径获取语言的名字（例如C:\zh-cn.resx 的语言名字是zh-cn）
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        private static string GetKey(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        /// <summary>
        /// 监视语言文件
        /// </summary>
        /// <param name="filePath">存放语言文件的物理路径</param>
        private void Watch(string filePath)
        {
            _watcher.Path = filePath;
            _watcher.IncludeSubdirectories = true;
            _watcher.Filter = "*" + ExtName;
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            /*如果有文件更新就更新相应的语言*/
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                string file = e.FullPath;
                if (File.Exists(file))
                {
                    ILanguage reader = new Language();
                    reader.Read(file);
                    string lang = GetKey(file);
                    if (!_langResources.ContainsKey(lang))
                        _langResources.Add(lang, reader);
                    else
                        _langResources[lang] = reader;
                }
            }
        }
    }

    /// <summary>
    /// 语言接口
    /// </summary>
    public interface ILanguage
    {
        /// <summary>
        /// 加载语言文件
        /// </summary>
        /// <param name="fileName">语言文件路径</param>
        void Read(string fileName);
        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="key">键</param> 
        string Get(string key);
    }

    /// <summary>
    /// 语言 
    /// </summary>
    public class Language : Dictionary<string, string>, ILanguage
    {
        /// <summary>
        /// 加载语言文件（这里语言文件是一般资源文件，其实只要是符合这样结构的xml文件就可以：
        ///    <data name="key"><value>文本</value></data>)
        /// </summary>
        /// <param name="fileName">语言文件路径</param>
        public void Read(string fileName)
        {
            var xe = XElement.Load(fileName);
            var xes = xe.Elements().Where(c => c.Name == "data" &&
                c.Attribute("name") != null && c.Element("value") != null).ToList();
            foreach (var x in xes)
            {
                var xElement = x.Element("value");
                if (xElement != null) Add(x.Attribute("name").Value, xElement.Value);
            }
        }

        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="key">键</param> 
        public string Get(string key)
        {
            return ContainsKey(key) ? this[key] : string.Format("[Language about {0} not found!]", key);
        }
    }
}
