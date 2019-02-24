using Dncblogs.Domain.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Dncblogs.Core
{
    public class BaseService
    {
        public readonly string DateTimeFormString = "yyyy-MM-dd HH:mm:ss";
        private DatabaseSetting databaseSetting;

        public BaseService(IOptions<DatabaseSetting> options)
        {
            databaseSetting = options.Value;
        }


        public DbConnection CreateConnection()
        {
            if (databaseSetting.DBType.ToUpper() == "MYSQL")
            {
                return new MySqlConnection(databaseSetting.ConnectionString);
            }
            else
            {
                return new SqlConnection(databaseSetting.ConnectionString);
            }
        }
    }
}
