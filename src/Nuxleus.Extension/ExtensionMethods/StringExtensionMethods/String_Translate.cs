using System;
using System.Text;

namespace Nuxleus {

    public static partial class StringExtensionMethods {

        /// <summary>
        /// Courtest of Dimitre Novatchev @ http://dnovatchev.spaces.live.com/blog/cns!44B0A32C2CCF7488!353.entry the Translate function
        /// mimics the same functionality provided by the XSLT translate function.
        /// </summary>
        /// <param name="source">
        /// Represents the string currently being operated upon.  This is not part of the extension signature.
        /// </param>
        /// <param name="replace">
        /// The string of characters to be replaced in the source string.
        /// </param>
        /// <param name="replaceWith">
        /// The string of characters to replace, based on character position as it relates to the replace string, 
        /// inside of the source string.
        /// </param>
        /// <returns>The translated string.</returns>
        public static string Translate ( this string source,
                             string replace, string replaceWith ) {
            StringBuilder sb
                 = new StringBuilder(source);

            int indPat = 0;
            int lrepImg = replaceWith.Length;

            foreach (char c in replace.ToCharArray()) {
                if (indPat < lrepImg)
                    sb.Replace(c, replaceWith[indPat]);
                else
                    sb.Replace(c.ToString(), "");

                indPat++;
            }
            return sb.ToString();
        }
    }
}
