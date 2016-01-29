using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbUtilsNET;
using System.Data.SqlClient;
using System.Collections.Generic;
using DbUtilsNET.POCO;

namespace UnitTestDbUtilsNET
{
    [TestClass]
    public class DbUtilsNETClientTest
    {
        private String getConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = "127.0.0.1";
            sqlBuilder.InitialCatalog = "VolkswagenCp3l2";
            sqlBuilder.UserID = "sa";
            sqlBuilder.Password = "50932458ztqh";
            sqlBuilder.ConnectTimeout = 1800;

            return sqlBuilder.ToString();
        }

        [TestMethod]
        public void TestInsert()
        {
            var id2 = -1;

            DbUtilsNETClient dc = new DbUtilsNETClient(getConnectionString());
            tblFISDataErr data = new tblFISDataErr(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "LSVN221Z8C2124094", "ERROR");
            data.nNo = 32;
            //插入单条
            id2 = Convert.ToInt32(dc.Insert(data));

            Assert.IsTrue(id2 != -1);
        }

        [TestMethod]
        public void TestUpdate()
        {
            DbUtilsNETClient dc = new DbUtilsNETClient(getConnectionString());
            tblFISDataErr data = new tblFISDataErr(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "LSVN221Z8C2000000", "ERROR2");

            IDictionary<string, Object> things = new Dictionary<String, Object>();
            things.Add("sReason", "ERROR");
            things.Add("nNo", "1");
            var res = dc.Update<tblFISDataErr>(data, things);

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void TestDelete()
        {
            DbUtilsNETClient dc = new DbUtilsNETClient(getConnectionString());
            tblFISDataErr data = new tblFISDataErr();
            data.sReason = "ERROR";
            data.nNo = 2;
            var res = dc.Delete<tblFISDataErr>(data);

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void TestQuery()
        {
            DbUtilsNETClient dc = new DbUtilsNETClient(getConnectionString());
            String sql = "SELECT * FROM tblFISDataErr WHERE sReason=@0";
            Object[] things = { "ERROR" };
            var res = dc.Query<tblFISDataErr>(sql, things);

            Assert.IsNotNull(res);
        }
    }
}
