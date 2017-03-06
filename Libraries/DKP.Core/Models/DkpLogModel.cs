using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Models
{
    public class DkpLogModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string DkpType { get; set; }
        public int? Score { get; set; }
        public string ExchangeName { get; set; }
        public string Remark { get; set; }
        public string CreatorName { get; set; }
        public string CreateTime { get; set; }
    }
}
