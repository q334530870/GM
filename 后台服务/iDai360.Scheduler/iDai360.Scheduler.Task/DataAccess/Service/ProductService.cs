using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iDai360.Scheduler.Task.Common;
using log4net;

namespace iDai360.Scheduler.Task.DataAccess.Service
{
    public class ProductService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void UpdatePrice()
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                try
                {
                    conn.Execute(@"
UPDATE [t_product_factor_rental] 
SET CurrentUnitPriceA = dbo.f_product_price_for_quotation(id,1,dbo.f_get_ledger_date(),all_in_irr_issue_A,case hesitation_period_hours when 0 then 'T0' else 'T1' end),
    CurrentUnitPriceB = dbo.f_product_price_for_quotation(id,2,dbo.f_get_ledger_date(),all_in_irr_issue_B,case hesitation_period_hours when 0 then 'T0' else 'T1' end)
WHERE IsStart = 1 and subscription_launch_date <= dbo.f_get_ledger_date() and subscription_close_date >= dbo.f_get_ledger_date()");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
    }
}
