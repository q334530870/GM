using DKP.Core.Data;

namespace DKP.Data
{
    public class ActivityItem : BaseEntity
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityId { get; set; }
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
        public int? Count { get; set; }
        /// <summary>
        /// 充值金额/登录天数
        /// </summary>
        public int? Amount { get; set; }
        /// <summary>
        /// 开始名次
        /// </summary>
        public int? StartRank { get; set; }
        /// <summary>
        /// 结束名次
        /// </summary>
        public int? EndRank { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 兑换次数
        /// </summary>
        public int? ExchangeCount { get; set; }
        /// <summary>
        /// 物品描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int? Number { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
