using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKP.Core.Models
{
    public class BillingsPrint
    {
        public string Code { get; set; }
        public string Number { get; set; }
        public string AccPeriod { get; set; }
        public string InvoiceType { get; set; }
        public string Invoice { get; set; }
        public string DjNumber { get; set; }
        public DateTime? DjDate { get; set; }
        public string LesseeNumber { get; set; }
        public string LesseeName { get; set; }
        public string LesseeTaxNumber { get; set; }
        public string LesseeAddressPhone { get; set; }
        public string lesseeBank { get; set; }
        public string Name { get; set; }

        public string Standard { get; set; }
        public string Unit { get; set; }
        public decimal? Count { get; set; }
        public decimal? Total { get; set; }
        public string vatRate { get; set; }
    }
}
