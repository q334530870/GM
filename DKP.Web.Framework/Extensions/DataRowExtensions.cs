using System.Data;

namespace DKP.Web.Framework.Extensions
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// 获取列数据
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string ToValue(this DataRow dataRow, string columnName)
        {
            if (dataRow == null) return null;

            return dataRow.Table.Columns.Contains(columnName) ? dataRow[columnName].ToString() : null;

        }
    }
}
