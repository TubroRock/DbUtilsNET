using DbUtilsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestDbUtilsNET.POCO
{
    // 日志信息
    public class LogInfoObject
    {
        /**************************************************/
        // 数据库信息
        public int nNo;                 // 记录标识（自动累加）
        public DateTime dtGetTime;      // 数据接收时间
        public string sLogger = "";     // 日志信息记录计算机
        public int nLogType = 0;        // 日志信息类型
        public string sLogInfo = "";    // 日志信息数据

        // 属性->(显示)数据绑定用
        [DbColumn(ColumnName = "nNo")]
        public int No { get { return nNo; } set { nNo = value; } }

        [DbColumn(ColumnName = "dtGetTime")]
        public DateTime GetTime { get { return dtGetTime; } set { dtGetTime = value; } }

        [DbColumn(ColumnName = "sLogger")]  
        public string Logger { get { return sLogger; } set { sLogger = value; } }
        [DbColumn(ColumnName = "nLogType")]
        public string LogType
        {
            get
            {
                //1:Info 2:Warn 3:Error 4:Fis
                string sLogType = "";

                switch (nLogType)
                {
                    case 0x01:
                        sLogType = "信息";
                        break;
                    case 0x02:
                        sLogType = "警告";
                        break;
                    case 0x03:
                        sLogType = "故障";
                        break;
                    case 0x04:
                        sLogType = "FIS通讯";
                        break;
                    default:
                        sLogType = "未知";
                        break;
                }
                return sLogType;
            }
            set { nLogType = Convert.ToInt32(value.ToString()); }
        }
        public string LogInfo { get { return sLogInfo; } }
    }
}
