using DKP.Data;

namespace DKP.Services.Security.Models
{
    public class LoginResult
    {
        /// <summary>
        /// 结果编号
        /// </summary>
        public LoginResultCode LoginResultCode { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public User User { get; set; }
    }

    /// <summary>
    /// 用户登录结果
    /// </summary>
    public enum LoginResultCode
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        Successful = 1,
        /// <summary>
        /// 用户不存在
        /// </summary>
        UserNotExist = 2,
        /// <summary>
        /// 密码错误
        /// </summary>
        WrongPassword = 3,
        /// <summary>
        /// 账号锁定
        /// </summary>
        Locked = 4,
        /// <summary>
        /// 其他错误
        /// </summary>
        Error = 5,
        /// <summary>
        /// 未配置角色
        /// </summary>
        NoRole = 6,
    }
}
