using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using DKP.Core;
using DKP.Core.Data;
using DKP.Core.Extensions;
using DKP.Core.Models;
using DKP.Services.Security.Models;
using User = DKP.Data.User;
using log4net;
using System.Reflection;
using DKP.Data;

namespace DKP.Services.Security
{
    public interface IUserService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isEncrypted">密码是否已加密</param>
        /// <returns></returns>
        LoginResult Login(string username, string password, bool isEncrypted = false);

        /// <summary>
        /// 根据Id获取用户对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User GetById(int id);

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User GetByUserName(string userName);

        /// <summary>
        /// 根据用户名或手机获取用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetByUserNameOrMobile(string username);

        /// <summary>
        /// 根据关键字检索用户
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        IList<User> SearchUser(string keyword);

        IPagedList<User> GetAllUsers(NameValueCollection forms, PageParam pageParam);
        PagedList<User> GetNoAdminUsers(NameValueCollection forms, PageParam pageParam);
        List<User> GetNoAdminUsers();
        User GetAdmin();
        void Register(User user, string sid);
        void AddUser(User user, int[] rid);
        void DeleteUser(int userId);
        User GetUser(int userId);
        void UserLeave(User user);
        void SetPwd(int id, string pwd);
        void SetRole(int id, string[] ids);
        void Update(User model);
        bool CheckNumber(string number);

        bool CheckUserName(string userName);

        bool CheckMobile(string mobile);

        bool CheckPassword(int uid, string password);

        void UpdateUser(User user, int[] rid);
        void UpdatePassword(int uid, string password);
    }

    public class UserService : IUserService
    {
        #region Constants

        private const string USER_ALLOWED_KEY = "DKP.user.{0}";

        private const string USER_PATTERN_KEY = "DKP.user.";

        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRoleService _roleService;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;

        #endregion

        #region Ctor

        public UserService(IRepository<User> userRepository,
            IEncryptionService encryptionService,
            IRoleService roleService, IDbContext dbContext,
            IDataProvider dataProvider)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _roleService = roleService;
            _dbContext = dbContext;
            _dataProvider = dataProvider;
        }

        #endregion

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">登录密码</param>
        /// <param name="isEncrypted">登录密码是否已经加密</param>
        /// <returns>用户信息</returns>
        public LoginResult Login(string username, string password, bool isEncrypted = false)
        {
            var loginResult = new LoginResult();

            if (string.IsNullOrEmpty(username))
            {
                loginResult.LoginResultCode = LoginResultCode.UserNotExist;
                loginResult.Message = "用户名不能为空";
                return loginResult;
            }

            if (string.IsNullOrEmpty(password))
            {
                loginResult.LoginResultCode = LoginResultCode.Error;
                loginResult.Message = "密码不能为空";
                return loginResult;
            }

            // 根据用户名获取数据
            var user =
                _userRepository.Table.FirstOrDefault(
                    t => t.UserName == username.Trim() || t.Telephone == username.Trim());

            if (user == null)
            {
                loginResult.LoginResultCode = LoginResultCode.UserNotExist;
                loginResult.Message = "用户不存在";
                return loginResult;
            }
            if (isEncrypted == false)
            {
                password = _encryptionService.CreatePasswordHash(password.Trim(), user.Salt);
            }
            if (password != user.Password)
            {
                loginResult.LoginResultCode = LoginResultCode.WrongPassword;
                loginResult.Message = "账号或密码错误";
                return loginResult;
            }

            if (!user.Roles.Any() && user.UserName != "admin")
            {
                loginResult.LoginResultCode = LoginResultCode.NoRole;
                loginResult.Message = "未配置角色";
                loginResult.User = user;
                return loginResult;
            }

            loginResult.LoginResultCode = LoginResultCode.Successful;
            loginResult.User = user;
            return loginResult;
        }
        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User GetByUserName(string userName)
        {
            return _userRepository.Table.FirstOrDefault(t => t.UserName == userName);
        }

        public User GetByUserNameOrMobile(string username)
        {
            return _userRepository.Table.FirstOrDefault(t => t.UserName == username || t.Telephone == username);
        }

        public IList<User> SearchUser(string keyword)
        {
            return
                _userRepository.TableNoTracking.Where(t=>
                   t.UserName.Contains(keyword)).ToList();
        }

        public User GetAdmin()
        {
            return _userRepository.Table.Single(u => u.UserName == "admin");
        }

        public IPagedList<User> GetAllUsers(NameValueCollection forms, PageParam pageParam)
        {
            var query = _userRepository.Table.Where(forms.ResolveToLinq()).OrderBy(u=>u.Number);
            return new PagedList<User>(query, pageParam);
        }

        public PagedList<User> GetNoAdminUsers(NameValueCollection forms, PageParam pageParam)
        {
            forms.Add("search_neq_UserName", "admin");
            var query = _userRepository.Table.Where(forms.ResolveToLinq()).OrderBy(u=>u.Number);
            return new PagedList<User>(query, pageParam);
        }

        public List<User> GetNoAdminUsers()
        {
            return _userRepository.Table.Where(u=>u.UserName!="admin").OrderBy(u => u.Number).ToList();
        }

        public void Register(User user, string sid)
        {
            user.Status = "普通用户";
            AddUser(user, null);
        }

        public void AddUser(User user, int[] rid)
        {
            using (var conn = _dbContext.GetDbConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        user.AddTime = DateTime.Now;
                        if (string.IsNullOrEmpty(user.Status))
                        {
                            user.Status = "在职";
                        }
                        user.Salt = _encryptionService.CreateSaltKey(5);
                        user.Password =
                            _encryptionService.CreatePasswordHash(
                                string.IsNullOrEmpty(user.Password) ? "123456" : user.Password,
                                user.Salt);
                        for(int i = 0; i < rid.Count(); i++)
                        {
                            var role = _roleService.GetRoleById(rid[i]);
                            user.Roles.Add(role);
                        }
                        user.MaxDkp = 0;
                        user.SpecialDkp = 0;
                        _userRepository.Insert(user);
                        _userRepository.Save();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        tran.Rollback();
                    }
                }               
            }
        }

        public void DeleteUser(int userId)
        {
            var user = GetById(userId);
            user.Roles.Clear();
            _userRepository.Delete(user);
            _userRepository.Save();
        }

        public User GetUser(int userId)
        {
            var user = _userRepository.GetById(userId);
            return user;
        }

        public void UserLeave(User user)
        {
            var model = _userRepository.GetById(user.Id);
            model.LeaveDate = user.LeaveDate;
            model.Status = "离职";
            _userRepository.Update(model);
            _userRepository.Save();
        }

        public void SetPwd(int id, string pwd)
        {
            var model = _userRepository.GetById(id);
            if (string.IsNullOrEmpty(model.Salt))
            {
                model.Salt = _encryptionService.CreateSaltKey(5);
            }
            model.Password = _encryptionService.CreatePasswordHash(pwd, model.Salt);
            _userRepository.Update(model);
            _userRepository.Save();
        }

        public void SetRole(int id, string[] ids)
        {
            var user = _userRepository.GetById(id);
            user.Roles.Clear();
            user.Roles = new List<Role>();
            if (ids != null)
            {
                foreach (var roleId in ids)
                {
                    if (string.IsNullOrEmpty(roleId)) continue;
                    user.Roles.Add(_roleService.GetRoleById(int.Parse(roleId)));
                }
            }
            _userRepository.Save();
        }

        public void Update(User model)
        {
            _userRepository.Update(model);
            _userRepository.Save();
        }

        public bool CheckNumber(string number)
        {
            return _userRepository.Table.Any(t => t.Number == number);
        }

        public bool CheckUserName(string userName)
        {
            return _userRepository.Table.Any(t => t.UserName == userName);
        }

        public bool CheckMobile(string mobile)
        {
            return _userRepository.Table.Any(t => t.Telephone == mobile);
        }

        public bool CheckPassword(int uid, string password)
        {
            var user = GetById(uid);
            return _encryptionService.CreatePasswordHash(password, user.Salt) == user.Password;
        }

        public void UpdateUser(User user, int[] rid)
        {
            
            if (string.IsNullOrEmpty(user.Password))
            {
                var old = GetById(user.Id);
                user.Password = old.Password;
            }
            else {
                user.Password = _encryptionService.CreatePasswordHash(user.Password,user.Salt);
            }
            _userRepository.Update(user);
            user.Roles.Clear();
            for (int i = 0; i < rid.Count(); i++)
            {
                var role = _roleService.GetRoleById(rid[i]);
                user.Roles.Add(role);
            }
            _userRepository.Save();
        }

        public void UpdatePassword(int uid, string password)
        {
            var user = GetById(uid);
            user.Password = _encryptionService.CreatePasswordHash(password, user.Salt);
            _userRepository.Update(user);
            _userRepository.Save();
        }
    }
}
