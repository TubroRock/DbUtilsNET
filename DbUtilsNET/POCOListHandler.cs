using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DbUtilsNET
{
    public class POCOListHandler : DbDataReaderHandler
    {
        private Type type;
        public POCOListHandler(Type type)
        {
            this.type = type;
        }

        public object Handler(System.Data.Common.DbDataReader reader)
        {
            List<Object> list = new List<Object>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    Object poco = asm.CreateInstance(type.ToString(), true);
                    int columnCount = reader.FieldCount;
                    // 遍历数据行，将数据封装到对应的类中
                    for (int i = 0; i < columnCount; i++)
                    {
                        try
                        {
                            // 获取列名
                            String columnName = reader.GetName(i);
                            // 获取对应值
                            Object columnData = reader.GetValue(i);
                            // 获取与该列对应的属性
                            PropertyInfo propertyInfo = type.GetProperty(columnName);
                            // 设置属性
                            propertyInfo.SetValue(poco, Convert.ChangeType(columnData, propertyInfo.PropertyType), null);
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }
                    }
                    list.Add(poco);
                }
            }

            return list;
        }
    }
}
