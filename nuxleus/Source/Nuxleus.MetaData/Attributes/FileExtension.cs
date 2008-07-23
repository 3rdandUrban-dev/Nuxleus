using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.MetaData {

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class FileExtensionAttribute : Attribute {

        public readonly string Extension;

        public FileExtensionAttribute(string extension) {
            Extension = extension;
        }

        public static string FromMember (object o) {
            return ((FileExtensionAttribute)
            o.GetType().GetMember(o.ToString())[0].GetCustomAttributes(typeof(FileExtensionAttribute), false)[0]).Extension;
        }

        public static string FromType (object o) {
            return ((FileExtensionAttribute)
            o.GetType().GetCustomAttributes(typeof(FileExtensionAttribute), false)[0]).Extension;
        }
    }
}

