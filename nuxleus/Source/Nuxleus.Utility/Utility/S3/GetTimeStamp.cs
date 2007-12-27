using System;
using System.Collections.Generic;
using System.Text;

namespace Extf.Net.Utility {
    public partial class S3 {
        public static DateTime GetTimeStamp(DateTime myTime) {
            DateTime myUniversalTime = myTime.ToUniversalTime();
            DateTime myNewTime = new DateTime(myUniversalTime.Year,
            myUniversalTime.Month, myUniversalTime.Day,
            myUniversalTime.Hour, myUniversalTime.Minute,
            myUniversalTime.Second, myUniversalTime.Millisecond);

            return myNewTime;
        }
    }
}
