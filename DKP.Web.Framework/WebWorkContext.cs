using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DKP.Core;
using DKP.Core.Extensions;
using DKP.Core.Models;
using DKP.Services.Security;

namespace DKP.Web.Framework
{
    public class WebWorkContext : IWorkContext
    {
        #region Const

        private const string UserSessionName = "DKP.user";
        private const string UserLanguageCookieName = "DKP.language";
        private const string UserPageSize = "DKP.pagesize";
        private const string UserSessionIdCookieName = "DKP.user.sid";
        private const string UserLoginInfoCookieName = "DKP.user";
        private const string UserLastActiveTimeCookieName = "DKP.lastactivetime";

        #endregion

        #region Fields

        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext, IPermissionService permissionService,
            IUserService userService, IEncryptionService encryptionService)
        {
            this._httpContext = httpContext;
            this._permissionService = permissionService;
            this._userService = userService;
            _encryptionService = encryptionService;
        }

        #endregion


        #region Utilities

        protected virtual User GetUserSession()
        {
            if (_httpContext == null || _httpContext.Session == null)
                return null;

            return _httpContext.Session[UserSessionName] as User;
        }

        protected virtual void SetUserSession(User userSession)
        {
            if (_httpContext != null && _httpContext.Session != null)
            {
                _httpContext.Session[UserSessionName] = userSession;
            }
        }

        protected virtual string GetLanguage()
        {
            if (_httpContext == null || _httpContext.Session == null) return string.Empty;
            var httpCookie = _httpContext.Request.Cookies[UserLanguageCookieName];
            return httpCookie != null ? httpCookie.Value : string.Empty;
        }

        protected virtual void SetLanaguage(string code)
        {
            HttpCookie cookie = new HttpCookie(UserLanguageCookieName)
            {
                Value = code
            };
            if (_httpContext != null && _httpContext.Session != null)
            {
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual string GetSidCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[UserSessionIdCookieName];
            if (cookie != null)
                return cookie.Value;

            return string.Empty;
        }

        protected virtual void SetSidCookie(string sid)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[UserSessionIdCookieName];
            if (cookie == null)
                cookie = new HttpCookie(UserSessionIdCookieName);

            cookie.Value = sid;
            cookie.Expires = DateTime.Now.AddDays(15);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        protected virtual string GetUserCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[UserLoginInfoCookieName];
            if (cookie != null)
                return cookie.Value;
            return string.Empty;
        }

        protected virtual void SetUserCookie(string logininfo)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[UserLoginInfoCookieName];
            if (cookie == null)
                cookie = new HttpCookie(UserLoginInfoCookieName);

            cookie.Value = logininfo;
            cookie.Expires = DateTime.Now.AddDays(7);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        protected virtual DateTime GetLastActiveTime()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[UserLastActiveTimeCookieName];
            if (cookie != null)
                return cookie.Value.ToDateTime();
            return DateTime.MinValue;
        }

        protected virtual void SetLastActiveTime(DateTime dateTime)
        {
            var cookie = HttpContext.Current.Request.Cookies[UserLastActiveTimeCookieName];
            if (cookie == null)
                cookie = new HttpCookie(UserLastActiveTimeCookieName);

            cookie.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            cookie.Expires = DateTime.Now.AddMinutes(15);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        #endregion

        public string SessionId
        {
            get { return GetSidCookie(); }
            set { SetSidCookie(value); }
        }

        public int CurrentUserId
        {
            get
            {
                var userSession = GetUserSession();
                if (userSession == null)
                {
                    return 0;
                }

                return userSession.UserId;
            }
            set { Reset(value); }
        }

        public User CurrentUser
        {
            get { return GetUserSession(); }
        }

        public string WorkingLanguage
        {
            get
            {
                var language = GetLanguage();
                if (string.IsNullOrEmpty(language))
                {
                    SetLanaguage("zh-cn");
                    language = "zh-cn";
                }

                return language;
            }
            set
            {
                SetLanaguage(value);
            }
        }

        public bool IsLogin
        {
            get { return GetUserSession() != null; }
        }

        public DateTime LastActiveTime
        {
            get { return GetLastActiveTime(); }
            set { SetLastActiveTime(value); }
        }

        public int PageSize
        {
            get
            {
                var httpCookie = _httpContext.Request.Cookies[UserPageSize];
                return httpCookie != null ? Convert.ToInt32(httpCookie.Value) : 20;
            }
            set
            {
                var cookie = new HttpCookie(UserPageSize)
                {
                    Value = value.ToString()
                };
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        public void Reset(int useId)
        {
            // 根据Id获取
            var user = _userService.GetById(useId);

            if (user != null && _httpContext != null && _httpContext.Session != null)
            {
                var us = new User
                {
                    Sid = _httpContext.Session.SessionID,
                    UserId = user.Id,
                    Name = user.UserName,
                    RealName = user.RealName,
                    Password = user.Password,
                    Mobile = user.Telephone,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Roles = user.Roles.Select(t => t.Name).ToArray(),
                    RoleIds = user.Roles.Select(t => t.Id).ToArray(),
                    Type = user.MemberType,
                    Permissions = new List<Permission>(),
                    IsSystemAdmin = user.IsSystem,
                    IsAllowEnterAdmin = user.Status == "在职" || string.IsNullOrEmpty(user.Status)
                };

                if (us.IsSystemAdmin)
                {
                    var permissions = _permissionService.GetAllPermissions();
                    foreach (var perm in permissions) { us.Permissions.Add(Mapper.Map<Data.Permission, Permission>(perm)); }
                }
                else
                {
                    var roles = user.Roles.ToList();
                    var permissions = new List<Data.Permission>();
                    foreach (var role in roles)
                    {
                        //使用union去除重复元素。
                        permissions = permissions.Union(role.Permissions).ToList();
                    }
                    permissions = permissions.OrderBy(t => t.Order).ToList();
                    foreach (var perm in permissions)
                    {
                        us.Permissions.Add(Mapper.Map<Data.Permission, Permission>(perm));
                    }
                }

                LastActiveTime = DateTime.Now;

                SetUserSession(us);
            }
        }

        public void Logout()
        {
            if (_httpContext.Session == null) return;
            _httpContext.Session.Clear();

            SetLastActiveTime(DateTime.MaxValue);
        }

        public bool CheckTimeout()
        {
            var timeSpan = (DateTime.Now - LastActiveTime).TotalMinutes;
            return timeSpan > 15;
        }
    }
}
