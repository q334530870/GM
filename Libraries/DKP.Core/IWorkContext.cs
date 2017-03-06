using DKP.Core.Models;
using System;

namespace DKP.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// 客户端唯一标示
        /// </summary>
        string SessionId { get; set; }

        /// <summary>
        /// 当前用户Id
        /// </summary>
        int CurrentUserId { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        User CurrentUser { get; }

        /// <summary>
        /// 当前选择的语言
        /// </summary>
        string WorkingLanguage { get; set; }

        /// <summary>
        /// 是否登陆
        /// </summary>
        bool IsLogin { get; }

        /// <summary>
        /// 每页显示数据
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 重新获取用户信息
        /// </summary>
        void Reset(int userId);

        /// <summary>
        /// 用户注销
        /// </summary>
        void Logout();

        /// <summary>
        /// 登录是否超市
        /// </summary>
        bool CheckTimeout();
    }
}
