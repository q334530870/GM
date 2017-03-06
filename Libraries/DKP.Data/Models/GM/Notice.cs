using DKP.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data
{
    public class Notice : BaseEntity
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 重复间隔（分钟）
        /// </summary>
        public int RepeatTime { get; set; }
        /// <summary>
        /// 开启方式（1：聊天频道 2：全频滚动）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 所选服务器ID
        /// </summary>
        public string ServerIds { get; set; }
        /// <summary>
        /// 显示内容
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 状态 1：成功 2：失败
        /// </summary>
        public int Status { get; set; }

    }
}
