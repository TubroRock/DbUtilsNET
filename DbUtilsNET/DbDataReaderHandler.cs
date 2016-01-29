using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DbUtilsNET
{
    public interface DbDataReaderHandler
    {
        /// <summary>
        /// 结果集处理方法
        /// </summary>
        /// <param name="reader">查询结果集</param>
        /// <returns></returns>
        Object Handler(DbDataReader reader);
    }
}
