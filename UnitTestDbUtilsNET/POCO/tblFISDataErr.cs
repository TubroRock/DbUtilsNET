using DbUtilsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestDbUtilsNET.POCO
{
    [DbTable("tblFISDataErr")]
    [DbResult]
    public class FISDataErr
    {
        public FISDataErr()
        {

        }

        public FISDataErr(String dtGetTime, String sFISData, String sReason)
        {
            this.dtGetTime = dtGetTime;
            this.sFISData = sFISData;
            this.sReason = sReason;
        }

        public int nNo { get; set; }
        public String dtGetTime { get; set; }
        public String sFISData { get; set; }
        public String sReason { get; set; }

        public override string ToString()
        {
            return nNo + "\t" + sFISData + "\t" + dtGetTime + "\t" + sReason + "\r\n";
        }
    }
}
