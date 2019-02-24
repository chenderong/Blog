using Dncblogs.Domain.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Dncblogs.Data.EntityFramework
{
    public static class DapperConFactory
    {
        /// <summary>
        /// 取数据库连接
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <returns></returns>
        public static DbConnection GetOpenConnection(DatabaseSetting databaseSetting)
        {
            if (databaseSetting.DBType.ToUpper() == "MYSQL")
            {
                var connection = new MySqlConnection(databaseSetting.ConnectionString);
                connection.Open();
                return connection;
            }
            else
            {
                var connection = new SqlConnection(databaseSetting.ConnectionString);
                connection.Open();
                return connection;
            }
        }
    }
}
