using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using DKP.Core.Data;
using System.Data;

namespace DKP.Data
{
    public interface IDbContext
    {
        void Modified<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>数据集</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体集合</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();

        /// <summary>
        /// 执行SQL查询，不包括关联数据
        /// </summary>
        /// <typeparam name="TElement">实体类型</typeparam>
        /// <param name="sql">Sql脚本</param>
        /// <param name="parameters">参数</param>
        /// <returns>Result</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable SqlQuery(string sql, params object[] parameters);

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isStoredProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable SqlQuery(string sql, bool isStoredProcedure,params object[] parameters);

        /// <summary>
        /// 对数据库执行给定的DDL和DML命令。
        /// </summary>
        /// <param name="sql">Sql脚本</param>
        /// <param name="doNotEnsureTransaction">是否使用事务</param>
        /// <param name="timeout">超时值,以秒为单位。null值表明,底层的提供者将使用的默认值</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响行数</returns>
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// 获取db连接
        /// </summary>
        /// <returns></returns>
        DbConnection GetDbConnection();
    }
}
