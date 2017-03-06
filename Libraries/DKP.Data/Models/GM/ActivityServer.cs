using DKP.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data
{
    public class ActivityServer : BaseEntity
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
