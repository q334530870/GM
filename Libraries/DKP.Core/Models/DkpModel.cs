using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Models
{
    public class DkpModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 总DKP
        /// </summary>
        public int? MaxDkp { get; set; }
        /// <summary>
        /// 消费DKP
        /// </summary>
        public int? CostDkp { get; set; }
        /// <summary>
        /// 核减DKP
        /// </summary>
        public int? MinusDkp { get; set; }
        /// <summary>
        /// 上季度DKP
        /// </summary>
        public int? PrevDkp { get; set; }
        /// <summary>
        /// 本季度DKP
        /// </summary>
        public int? CurrentDkp { get; set; }
        /// <summary>
        /// 特殊DKP
        /// </summary>
        public int? SpecialDkp { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string Number { get; set; }
    }
}
