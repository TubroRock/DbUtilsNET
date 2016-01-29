using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DbUtilsNET
{
    public class DbUtilsNETTool
    {
        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlParameter[] GetParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                // 当前实例类型
                var type = obj.GetType();
                // 所有公共属性
                var propertiesObj = type.GetProperties();
                foreach (PropertyInfo r in propertiesObj)
                {
                    var value = r.GetValue(obj, null);
                    if (value == null) value = DBNull.Value;
                    listParams.Add(new SqlParameter("@" + r.Name, value.ToString()));
                }
            }
            return listParams.ToArray();
        }
    }
}
