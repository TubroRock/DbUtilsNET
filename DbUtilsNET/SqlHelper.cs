using System;
using System.Collections.Generic;
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

        public SqlHelper()
        {

        }

        public SqlHelper(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return this.conn;
        }

        public object ExcuteSql(String strSql, object pars)
        {
            return ExcuteSql(strSql, DbUtilsNETTool.GetParameters(pars));
        }

        public object ExcuteSql(String strSql, params SqlParameter[] pars)
        {
            SqlConnection conn = null;
            SqlCommand sqlCommand = new SqlCommand();

            try
            {
                using (conn = this.GetConnection())
                {
                    sqlCommand.Connection = conn;
                    conn.Open();
                    sqlCommand.CommandText = strSql;
                    sqlCommand.Parameters.AddRange(pars);
                    object scalar = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Parameters.Clear();
                    return scalar;
                }
            }
            catch (Exception)
            {
                //Log the Exception
                return null;
            }
            finally
            {
                Close(conn);
            }
        }

        /// <summary>
        /// Executes a series of DBCommands in a transaction
        /// </summary>
        public virtual int Execute(IEnumerable<DbCommand> commands)
        {
            var result = 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var cmd in commands)
                    {
                        try
                        {
                            cmd.Connection = conn;
                            cmd.Transaction = tx;
                            Console.WriteLine(cmd.CommandText);
                            result += cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    tx.Commit();
                }
            }
            return result;
        }

        public DbDataReader GetReader(DbCommand cmd)
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
                conn.Close();
        }
    }
}
