using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Utility.Format
{
    public class Base64FormatProvider : ICustomFormatter, IFormatProvider
    {
        public Base64FormatProvider() { }

        Encoding encode = new UTF8Encoding();

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format != null)
                return Convert.ToBase64String(encode.GetBytes(format.ToCharArray()));
            else if (arg != null)
                return ((IFormattable)arg).ToString(format, formatProvider);
            else return null;
        }
    }
}
