using System;
using System.Diagnostics;
using System.Text;
using Xameleon.Utility.S3;

namespace Xameleon.Function {

    public static class S3ObjectCompare {

        public static bool Compare(AWSAuthConnection connect, String bucket, String key, String compareTo) {
            bool compare = connect.get(bucket, key).Object.Data == compareTo;
            if (compare) {
                return true;
            } else {
                Debug.Assert(compare);
                return false;
            }         
        }

        public static string DebugCompare(AWSAuthConnection connect, String bucket, String key, String compareTo) {
            StringBuilder builder = new StringBuilder();
            String objectValue = connect.get(bucket, key).Object.Data;
            builder.AppendLine("bucket: " + bucket);
            builder.AppendLine("key: " + bucket);
            builder.AppendLine("object value: " + objectValue);
            builder.AppendLine("compare: " + compareTo);

            if (objectValue == compareTo) {
                builder.AppendLine("True: The objects are the same");
            } else {
                builder.AppendLine("False: The objects are not the same");
            }
            return builder.ToString();
        }

    }
}
