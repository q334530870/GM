using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DKP.Core.Models
{
    public class ExchangeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Score { get; set; }
        public string Remark { get; set; }
        public string DkpType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CreateDate { get; set; }
        public string CreatorName { get; set; }
    }
}
