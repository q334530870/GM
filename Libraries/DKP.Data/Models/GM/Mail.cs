using DKP.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data
{
    public class Mail : BaseEntity
    {
        public Mail()
        {
            this.MailItems = new List<MailItem>();
        }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string CharacterId { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 状态 1：成功 0：失败
        /// </summary>
        public int Status { get; set; }
        public virtual ICollection<MailItem> MailItems { get; set; }
    }
}
