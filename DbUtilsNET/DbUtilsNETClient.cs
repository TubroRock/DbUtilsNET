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
        DbProviderFactory _factory;

        public DbUtilsNETClient(string connectionString)
        {
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            this.conn = new SqlConnection(connectionString);
        }
        public object Insert<T>(T entity) where T : class
        {
            var commands = new List<DbCommand>();
            var command = CreateInsertCommand(entity);
            commands.Add(command);
            return Execute(commands);
        }

        public object Update<T>(T entity, IDictionary<string, Object> things) where T : class
        {
            var commands = new List<DbCommand>();
            var command = CreateUpdateCommand(entity, things);
            commands.Add(command);
            return Execute(commands);
        }

        public object Delete<T>(T entity) where T : class
        {
            var commands = new List<DbCommand>();
            var command = CreateDeleteCommand(entity);
            commands.Add(command);
            return Execute(commands);
        }

        public List<T> Query<T>(String sql, Object[] things) where T : class, new()
        {
            IDataReader reader = ExecuteReader(CreateCommand(sql, null, things));
            return reader.Select<T>();
        }

        /// <summary>
        /// Creates a DBCommand that you can use for loving your database.
        /// </summary>
        DbCommand CreateCommand(string sql, DbConnection conn, params object[] args)
        {
            var result = _factory.CreateCommand();
            result.Connection = conn;
            result.CommandText = sql;
            if (args.Length > 0)
                result.AddParams(args);
            return result;
        }

        /// <summary>
        /// Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        public virtual DbCommand CreateInsertCommand(object o)
        {
            DbCommand result = null;
            String tableName = String.Empty;

            var expando = o.ToExpando();
            var settings = (IDictionary<string, object>)expando;
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();
            var stub = "INSERT INTO {0} ({1}) \r\n VALUES ({2})";
            result = CreateCommand(stub, null);
            int counter = 0;
            foreach (var item in settings)
            {
                sbKeys.AppendFormat("{0},", item.Key);
                sbVals.AppendFormat("@{0},", counter.ToString());
                result.AddParam(item.Value);
                counter++;
            }
            if (counter > 0)
            {
                // 去掉空格
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
                var vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
                // 构建SQL语句
                tableName = o.GetType().Name;
                var sql = string.Format(stub, tableName, keys, vals);
                result.CommandText = sql;
            }
            else throw new InvalidOperationException("Can't parse this object to the database - there are no properties set");
            return result;
        }

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        public virtual DbCommand CreateDeleteCommand(object o)
        {
            DbCommand result = null;
            String tableName = String.Empty;

            var expando = o.ToExpando();
            var settings = (IDictionary<string, object>)expando;
            var sbVals = new StringBuilder();
            var stub = "DELETE FROM {0} WHERE {1}";
            result = CreateCommand(stub, null);
            int counter = 0;
            foreach (var item in settings)
            {
                var val = item.Value;
                // 非主键和非空的值才会被更新
                if (item.Value != null)
                {
                    result.AddParam(val);
                    sbVals.AppendFormat("{0} = @{1} AND \r\n", item.Key, counter.ToString());
                    counter++;
                }
            }
            if (counter > 0)
            {
                // 去掉最后多于的AND
                var vals = sbVals.Remove(sbVals.Length - 7, 7);
                tableName = o.GetType().Name;
                result.CommandText = string.Format(stub, tableName, vals);
            }
            else throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
            return result;
        }

        /// <summary>
        /// Creates a update command for use with transactions
        /// </summary>
        public virtual DbCommand CreateUpdateCommand(object o, IDictionary<string, Object> things)
        {
            DbCommand result = null;
            String tableName = String.Empty;
            String PrimaryKeyField = String.Empty;

            var expando = o.ToExpando();
            var settings = (IDictionary<string, object>)expando;
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();
            var stub = "UPDATE {0} SET {1} WHERE {2}";
            var args = new List<object>();
            result = CreateCommand(stub, null);
            int counter = 0;
            foreach (var item in settings)
            {
                var val = item.Value;
                // 非主键和非空的值才会被更新
                if (!item.Key.Equals(PrimaryKeyField, StringComparison.CurrentCultureIgnoreCase) && item.Value != null)
                {
                    result.AddParam(val);
                    sbKeys.AppendFormat("{0} = @{1}, \r\n", item.Key, counter.ToString());
                    counter++;
                }
            }
            if (counter > 0)
            {
                foreach (KeyValuePair<String, Object> item in things)
                {
                    var val = item.Value;
                    if (item.Value != null)
                    {
                        result.AddParam(item.Value);
                        sbVals.AppendFormat("{0} = @{1} AND \r\n", item.Key, counter.ToString());
                        counter++;
                    }
                }
            }
            if (counter > 0)
            {
                // 去掉逗号
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);
                // 去掉最后多于的AND
                var vals = sbVals.Remove(sbVals.Length - 7, 7);
                tableName = o.GetType().Name;
                result.CommandText = string.Format(stub, tableName, keys, vals);
            }
            else throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
            return result;
        }
    }
}
