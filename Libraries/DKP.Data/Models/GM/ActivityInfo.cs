using DKP.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data
{
    public class ActivityInfo : BaseEntity
    {
        public ActivityInfo()
        {
            this.Activitys = new List<Activity>();
        }
        /// <summary>
        /// 活动分类（1：充值类 2：消耗类 3：登录类 4：连续登录类）
        /// </summary>
        public string ActType { get; set; }
        /// <summary>
        /// 对应活动类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 奖励名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        public string Desc { get; set; }
        /// <summary>
        /// 状态 1：成功 0：失败
        /// </summary>
        public int Status { get; set; }
        public virtual ICollection<Activity> Activitys { get; set; }
    }
}
