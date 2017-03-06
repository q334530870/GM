using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using log4net;

namespace iDai360.Scheduler.Task.DataAccess
{
    public static class SqlConnectionHelper
    {

        /// <summary>
        /// 日志记录
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 根据数据库连接名称获取SqlConnection
        /// </summary>
        /// <param name="name">App.config文件中的链接名称</param>
        /// <returns>SqlConnection</returns>
        public static SqlConnection GetConnection(string name)
        {
            SqlConnection connection = null;
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                connection = new SqlConnection(connStr);
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                
            }

            return connection;
        }

    }
}
