using DKP.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Data
{
    public class MailItem : BaseEntity
    {
        /// <summary>
        /// 邮件ID
        /// </summary>
        public int MailId { get; set; }
        /// <summary>
        /// 物品类型ID
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 物品类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        public virtual Mail Mail { get; set; }
    }
}
