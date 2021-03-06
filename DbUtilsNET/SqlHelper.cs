﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbUtilsNET
{
    public class SqlHelper
    {
        // 数据连接池
        protected SqlConnection conn;
        private DbProviderFactory _factory;

        public SqlHelper()
        {

        }

        public SqlHelper(string connectionString)
        {
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            this.conn = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 一个有效的数据库连接对象
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            return this.conn;
        }

        #region CreateCommand
        /// <summary>
        /// Creates a DBCommand that you can use for loving your database.
        /// </summary>
        public DbCommand CreateCommand(string sql, DbConnection conn, params object[] args)
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
        public virtual DbCommand CreateInsertCommand(object o, string tbName="")
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
                if (tbName == "")
                    tableName = o.GetType().Name;
                else
                    tableName = tbName;
                var sql = string.Format(stub, tableName, keys, vals);
                result.CommandText = sql;
            }
            else throw new InvalidOperationException("Can't parse this object to the database - there are no properties set");
            return result;
        }

        /// <summary>
        /// Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        public virtual DbCommand CreateDeleteCommand(object o, string tbName="")
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
                if (tbName == "")
                    tableName = o.GetType().Name;
                else
                    tableName = tbName;
                result.CommandText = string.Format(stub, tableName, vals);
            }
            else throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
            return result;
        }

        /// <summary>
        /// Creates a update command for use with transactions
        /// </summary>
        public virtual DbCommand CreateUpdateCommand(object o, IDictionary<string, Object> things, string tbName="")
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
                if (tbName == "")
                    tableName = o.GetType().Name;
                else
                    tableName = tbName;
                result.CommandText = string.Format(stub, tableName, keys, vals);
            }
            else throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
            return result;
        }
        #endregion

        public object ExecuteNonQuery(String strSql, object pars)
        {
            return ExecuteNonQuery(strSql, DbUtilsNETTool.GetParameters(pars));
        }

        public object ExecuteNonQuery(String strSql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand();

            sqlCommand.Connection = conn;
            conn.Open();
            sqlCommand.CommandText = strSql;
            sqlCommand.Parameters.AddRange(pars);

            return ExecuteNonQuery(sqlCommand);
        }

        /// <summary>
        /// 在一个事务中执行DbCommand
        /// </summary>
        /// <param name="command">处理过DbCommand</param>
        /// <returns>受影响的行数</returns>
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            return ExecuteNonQuery(new DbCommand[] { command }, true);
        }

        public virtual int ExecuteNonQuery(IEnumerable<DbCommand> commands)
        {
            return ExecuteNonQuery(commands, true);
        }

        /// <summary>
        /// 在一个事务中执行多条DbCommand
        /// </summary>
        /// <param name="commands">处理过的多条DbCommand的集合</param>
        /// <returns>受影响的行数</returns>
        public virtual int ExecuteNonQuery(IEnumerable<DbCommand> commands, bool closeConn)
        {
            var result = 0;
            var conn = GetConnection();

            try
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = tx;
                        Console.WriteLine(cmd.CommandText);
                        result += cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
            catch (DbException ex)
            {
                throw;
            }
            finally
            {
                if(closeConn)
                {
                    Close(conn);
                }
            }

            return result;
        }

        public IDataReader ExecuteReader(DbCommand cmd)
        {
            SqlConnection conn = GetConnection();//返回DataReader时,是不可以用using()的
            try
            {
                cmd.Connection = conn;
                conn.Open();
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);//关闭关联的Connection
            }
            catch (DbException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 关闭连接池
        /// </summary>
        public void Close(SqlConnection conn)
        {
            if (conn != null)
            {
                conn.Close();
            }
        }
    }
}
