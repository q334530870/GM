using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using iDai360.Scheduler.Task.Common;
using iDai360.Scheduler.Task.DataAccess.Model.Subscription;
using log4net;

namespace iDai360.Scheduler.Task.DataAccess.Service
{
    public class SubscriptionService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Confirm()
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        /**
                            1、检查当前ledgerdate下是否有数据，没有就退出
                            2、上一步查询时取得id，调用sp
                            3、如果不报错，就更新status   
                        */
                        var dailyInterestAccu = conn.Query<DailyInterestAccu>(
                            "select id,status from [dbo].[t_daily_interest_accu] where ledger_date = [dbo].[f_get_ledger_date]()",
                            transaction: tran).FirstOrDefault();
                        int id;
                        if (dailyInterestAccu == null)
                        {
                            var result = conn.ExecuteScalar(
                               "INSERT INTO [dbo].[t_daily_interest_accu] ([is_processed_thru], [ledger_date], [status]) VALUES (0,dbo.f_get_ledger_date(),0);" +
                               "SELECT SCOPE_IDENTITY();", null, tran);
                            id = Convert.ToInt32(result);
                        }
                        else if (dailyInterestAccu.status == 1)
                        {
                            return;
                        }
                        else
                        {
                            id = dailyInterestAccu.id;
                        }

                        conn.Execute(
                            "update [dbo].[t_daily_interest_accu] set status = 1 where id = @daily_interest_accu_id",
                            new { daily_interest_accu_id = id }, tran);

                        tran.Commit();
                        Logger.Info("subscription confirm success");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        tran.Rollback();
                    }
                }
            }
        }

        public static void ConditionalityExpiryCheck()
        {
            using (var conn = SqlConnectionHelper.GetConnection(Constant.Idai360DbName))
            {
                try
                {
                    conn.Execute("dbo.sp_conditionality_expiry_check_run", commandType: CommandType.StoredProcedure);
                    Logger.Info("subscription conditionality_expiry_check success");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
    }
}
