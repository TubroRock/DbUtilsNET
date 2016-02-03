using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbUtilsNET
{
    public class DbUtilsNETClient : SqlHelper
    {
        public DbUtilsNETClient(string connectionString):base(connectionString)
        {
        }
        public object Insert<T>(T entity) where T : class, new()
        {
            var command = CreateInsertCommand(entity);
            return ExecuteNonQuery(command);
        }

        public object Update<T>(T entity, IDictionary<string, Object> things) where T : class, new()
        {
            var command = CreateUpdateCommand(entity, things);
            return ExecuteNonQuery(command);
        }

        public object Delete<T>(T entity) where T : class, new()
        {
            var command = CreateDeleteCommand(entity);
            return ExecuteNonQuery(command);
        }

        public List<T> Query<T>(String sql, Object[] things) where T : class, new()
        {
            IDataReader reader = ExecuteReader(CreateCommand(sql, null, things));
            try
            {
                return reader.Select<T>();
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }
        }

        public int Update(String sql, Object[] things)
        {
            var command = CreateCommand(sql, null, things);
            return ExecuteNonQuery(command);
        }
    }
}
