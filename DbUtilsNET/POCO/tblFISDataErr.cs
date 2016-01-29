using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbUtilsNET.POCO
{
    public class tblFISDataErr
    {
        public tblFISDataErr()
        {

        }

        public tblFISDataErr(String dtGetTime, String sFISData, String sReason)
        {
            this.dtGetTime = dtGetTime;
            this.sFISData = sFISData;
            this.sReason = sReason;
        }

        public int nNo { get; set; }
        public String dtGetTime { get; set; }
        public String sFISData { get; set; }
        public String sReason { get; set; }
    }
}