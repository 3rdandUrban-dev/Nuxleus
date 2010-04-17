using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.MetaData {

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class MessageAttribute : Attribute {

        public readonly string Content;

        public MessageAttribute (string message) {
            Content = message;
        }

        public static string FromMember (object o) {
            return ((MessageAttribute)
            o.GetType().GetMember(o.ToString())[0].GetCustomAttributes(typeof(MessageAttribute), false)[0]).Content;
        }

        public static string FromType (object o) {
            return ((MessageAttribute)
            o.GetType().GetCustomAttributes(typeof(MessageAttribute), false)[0]).Content;
        }
    }
}

